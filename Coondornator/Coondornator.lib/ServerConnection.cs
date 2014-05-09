using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;

namespace Compass.Coondornator
{
    public class ServerConnection : IDisposable
    {
        private SshClient _ssh;
        private SftpClient _sftp;
        public string UserName { get; set; }
        public string Host { get; set; }
        public bool IsConnected { get; private set; }
        private bool _disposed;

        public ServerConnection(string hostURL, string username)
        {
            UserName = username;
            Host = hostURL;
            _disposed = false;
            IsConnected = false;
        }

        public Task<bool> ConnectAsync(string password)
        {
            Task<bool> task = new Task<bool>(() =>
            {
                try
                {

                    SecureString securePasswod = DecryptString(password);
                    _sftp = new SftpClient(Host, UserName, ToInsecureString(securePasswod));
                    _sftp.OperationTimeout = new TimeSpan(0, 0, 10);
                    _sftp.Connect();
                    
                    _ssh = new SshClient(Host, UserName, ToInsecureString(securePasswod));
                    _ssh.Connect();

                    IsConnected = true;
                    return true;
                }
                catch (Exception)
                {
                    IsConnected = false;
                    return false;
                }
            });
            task.Start();
            return task;
        }

        public Task<IEnumerable<DatabaseFile>> GetBlastDatabases()
        {
            Task<IEnumerable<DatabaseFile>> task = new Task<IEnumerable<DatabaseFile>>(
                () => ListDirectory(Coondornator.CondorDatabaseDirectory).Where(f => Path.GetExtension(f).Equals(".pin")).Select(f => new DatabaseFile(f))
                );
            task.Start();
            return task;
        }

        public IEnumerable<string> ListDirectory(string remoteDirectory)
        {
            return from file in _sftp.ListDirectory(remoteDirectory)
                   select file.FullName;          
        }               

        public event EventHandler<ProgressEventArgs> UploadProgress;
        public event EventHandler<FileUploadEventArgs> UploadStart;

        private void OnUploadProgress(long position, long length)
        {
            var handler = UploadProgress;
            if (handler != null)
            {
                handler(this, new ProgressEventArgs(position, length));
            }
        }

        private void OnUploadStart(string fileName)
        {
            var handler = UploadStart;
            if (handler != null)
            {
                handler(this, new FileUploadEventArgs(fileName));
            }
        }

        public Task PutFileAsync(File file, string remoteDirectory, string destName = "")
        {
            if (!remoteDirectory.EndsWith("/"))
                remoteDirectory += "/";

            string filePath = remoteDirectory + (string.IsNullOrWhiteSpace(destName) ? file.NameWithExtension : destName);
            Task t = new Task(() =>
            {
                using (Stream stream = System.IO.File.OpenRead(file.FilePath))
                {
                    long length = stream.Length;
                    _sftp.UploadFile(stream, filePath, (count) => OnUploadProgress((long)count, length));
                }
            });
            t.Start();
            return t;
        }

        public Task PutFilesAsync(IEnumerable<File> files, string remoteDirectory)
        {
            if (!remoteDirectory.EndsWith("/"))
                remoteDirectory += "/";
            
            Task t = new Task(() =>
            {
                foreach (File file in files)
                {
                    string filePath = remoteDirectory +  file.NameWithExtension;
                    OnUploadStart(filePath);
                    using (Stream stream = System.IO.File.OpenRead(file.FilePath))
                    {
                        long length = stream.Length;
                        _sftp.UploadFile(stream, filePath, (count) => OnUploadProgress((long)count, length));
                    }
                }
            });
            t.Start();
            return t;
        }

        public string CreateDirectory(string remoteDirectory)
        {
            if (_sftp.Exists(remoteDirectory))
            {
                throw new DuplicateNameException("The directory " + remoteDirectory + " already exists on the server, please enter a unique name");
            }
            _sftp.CreateDirectory(remoteDirectory);
            if (!remoteDirectory.EndsWith("/"))
                remoteDirectory += "/";
            return remoteDirectory;
        }

        public string RunSubmission(string remoteJobDirectory, string submitFileName)
        {
            string completePath = remoteJobDirectory + submitFileName;

            SshCommand command = _ssh.RunCommand("cd " + remoteJobDirectory + ";" + Coondornator.CondorSubmitExecutable + " " + completePath);
       
            if (command.ExitStatus != 0)
            {
                throw new Exception("Unable to do condor_submit! " + command.Error);
            }

            return command.Result;
        }

        public string CondorFolder
        {
            get { return string.Format("/home/{0}/Condor/", UserName); }
        }

        public void Dispose()
        {
            Dispose(true);
          
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {            
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_sftp != null)
                    {
                        _sftp.Disconnect();
                        _sftp.Dispose();
                    }                      
                }
                _sftp = null;
                _disposed = true;
            }
        }

        static readonly byte[] entropy = Encoding.Unicode.GetBytes("Throwdown n -Joshua Coon");

        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
        
       
    }
}

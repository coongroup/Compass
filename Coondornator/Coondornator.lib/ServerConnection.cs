using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.IO;

namespace Compass.Coondornator
{
    public class ServerConnection : IDisposable
    {
        private SftpClient _sftp;
        private string _userName;
        private bool _disposed;

        public ServerConnection(string hostURL, string username, string password)
        {
            _sftp = new SftpClient(hostURL, username, password);
            _sftp.Connect();
            _disposed = false;
            _userName = username;
        }

        public IEnumerable<string> GetBlastDatabases()
        {
            return from db in ListDirectory(Coondornator.CondorDatabaseDirectory)
                   where Path.GetExtension(db).Equals(".pin")
                   select Path.GetFileNameWithoutExtension(db);             
        }

        public IEnumerable<string> ListDirectory(string remoteDirectory)
        {  
            return from file in _sftp.ListDirectory(remoteDirectory)
                   select file.FullName;          
        }               

        public void PutFile(File file, string remoteDirectory)
        {
            using (Stream stream = System.IO.File.OpenRead(file.FilePath))
            {
                _sftp.UploadFile(stream, remoteDirectory);
            }           
        }

        private string CondorFolder
        {
            get { return string.Format("/home/{0}/Condor/", _userName); }
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

    
    }
}

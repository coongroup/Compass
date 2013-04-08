using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tamir.SharpSsh;

namespace Compass.Coondornator
{
    public class ServerConnection : IDisposable
    {
        private Scp _scp;
        private string _userName;
        private bool _disposed;

        public ServerConnection(string hostURL, string username, string password)
        {
            _scp = new Scp(hostURL, username, password);            
            _disposed = false;
            _userName = username;
        }

        public void PutFile(File file, string remoteDirectory)
        {
            _scp.Put(file.FilePath, CondorFolder + remoteDirectory);
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
                    if (_scp != null)                    
                        _scp.Close();   
                }
                _scp = null;
                _disposed = true;
            }
        }

    
    }
}

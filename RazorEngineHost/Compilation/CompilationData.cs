namespace RazorEngineHost.Compilation
{
    using System;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;

    /// <summary>
    /// Provides (temporary) data about an compilation process.
    /// </summary>
    public class CompilationData : IDisposable
    {
        private bool disposed;

        /// <summary>
        /// The generated source code of the template (can be null).
        /// </summary>
        public string SourceCode { get; }

        /// <summary>
        /// returns the temporary folder for the compilation process (can be null).
        /// </summary>
        public string TmpFolder { get; }

        /// <summary>Creates a new CompilationData instance.</summary>
        /// <param name="sourceCode">The generated source code for the template.</param>
        /// <param name="tmpFolder">The temporary folder for the compilation process</param>
        public CompilationData(string sourceCode, string tmpFolder)
        {
            this.TmpFolder = tmpFolder;
            this.SourceCode = sourceCode;
        }

        /// <summary>Destructs the current instance.</summary>
        ~CompilationData()
        {
            this.Dispose(false);
        }

        /// <summary>Deletes all remaining files</summary>
        [SecuritySafeCritical]
        public void DeleteAll()
        {
            new PermissionSet(PermissionState.Unrestricted).Assert();
            if (this.TmpFolder == null)
                return;
            try
            {
                foreach (string enumerateFile in Directory.EnumerateFiles(this.TmpFolder))
                {
                    try
                    {
                        File.Delete(enumerateFile);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            try
            {
                foreach (string enumerateDirectory in Directory.EnumerateDirectories(this.TmpFolder))
                {
                    try
                    {
                        Directory.Delete(enumerateDirectory, true);
                    }
                    catch (IOException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            try
            {
                Directory.Delete(this.TmpFolder, true);
            }
            catch (IOException)
            { 
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>Clean up the compilation (ie delete temporary files).</summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        /// <summary>Cleans up the data of the current compilation.</summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            this.DeleteAll();
            this.disposed = true;
        }
    }
}
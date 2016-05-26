namespace RazorEngineHost.Compilation.ReferenceResolver
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>Represents a reference for the compiler</summary>
    public abstract class CompilerReference
    {
        /// <summary>The type of the current reference.</summary>
        public CompilerReferenceType ReferenceType { get; private set; }

        internal CompilerReference(CompilerReferenceType assemblyReference)
        {
            this.ReferenceType = assemblyReference;
        }

        /// <summary>execute the given visitor.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public abstract T Visit<T>(ICompilerReferenceVisitor<T> visitor);

        /// <summary>Create a compiler reference from the given file.</summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static CompilerReference From(string file)
        {
            return new FileReference(file);
        }

        /// <summary>
        /// Create a compiler reference from the given assembly.
        /// NOTE: The CodeDom compiler doesn't support assembly references where assembly.Location is null (roslyn only)!
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static CompilerReference From(Assembly assembly)
        {
            return new DirectAssemblyReference(assembly);
        }

        /// <summary>
        /// Create a compiler reference from the given stream.
        /// NOTE: The CodeDom compiler doesn't support stream references (roslyn only)!
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static CompilerReference From(Stream stream)
        {
            return new StreamReference(stream);
        }

        /// <summary>
        /// Create a compiler reference from the given byte array.
        /// NOTE: The CodeDom compiler doesn't support byte array references (roslyn only)!
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static CompilerReference From(byte[] byteArray)
        {
            return new ByteArrayReference(byteArray);
        }

        /// <summary>
        /// Try to resolve the reference to a file (throws when this is not possible).
        /// </summary>
        /// <param name="exceptionCreator"></param>
        /// <returns></returns>
        public string GetFile(Func<string, Exception> exceptionCreator = null)
        {
            return
                this.Visit(
                    new SelectFileVisitor(exceptionCreator));
        }

        private static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
                return true;
            if (a1 == null || a2 == null || a1.Length != a2.Length)
                return false;
            return !a1.Where((t, index) => t != a2[index]).Any();
        }

        /// <summary>
        /// Checks if the given object is equal to the current object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var compilerReference = obj as CompilerReference;
            if (compilerReference == null)
                return false;
            var assemblyReference1 =
                this as DirectAssemblyReference;
            if (assemblyReference1 != null)
            {
                var assembly = assemblyReference1.Assembly;
                var assemblyReference2 =
                    compilerReference as DirectAssemblyReference;
                if (assemblyReference2 != null)
                    return assembly == assemblyReference2.Assembly;
                var fileReference = compilerReference as FileReference;
                if (fileReference != null)
                    return fileReference.File == assembly.Location;
                return false;
            }
            var fileReference1 = this as FileReference;
            if (fileReference1 != null)
            {
                var file = fileReference1.File;
                var fileReference2 = compilerReference as FileReference;
                if (fileReference2 != null)
                    return fileReference2.File == file;
                var assemblyReference2 =
                    compilerReference as DirectAssemblyReference;
                if (assemblyReference2 != null)
                    return file == assemblyReference2.Assembly.Location;
            }
            var byteArrayReference1 = this as ByteArrayReference;
            if (byteArrayReference1 != null)
            {
                var byteArrayReference2 =
                    compilerReference as ByteArrayReference;
                if (byteArrayReference2 != null)
                    return ByteArrayCompare(
                        byteArrayReference1.ByteArray,
                        byteArrayReference2.ByteArray);
            }
            return false;
        }

        /// <summary>Gets a hashcode for the current object.</summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var assemblyReference =
                this as DirectAssemblyReference;
            if (assemblyReference?.Assembly.Location != null)
                return assemblyReference.Assembly.Location.GetHashCode();
            var fileReference = this as FileReference;
            if (fileReference != null)
                return fileReference.File.GetHashCode();
            var byteArrayReference = this as ByteArrayReference;
            if (byteArrayReference != null)
                return byteArrayReference.ByteArray.GetHashCode();
            var streamReference = this as StreamReference;
            if (streamReference != null)
                return streamReference.Stream.GetHashCode();
            throw new InvalidOperationException("Unknown CompilerReference!");
        }

        /// <summary>
        /// Visitor pattern for the <see cref="ReferenceResolver.CompilerReference" /> class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface ICompilerReferenceVisitor<out T>
        {
            /// <summary>Handle a direct assembly reference</summary>
            /// <param name="assembly"></param>
            /// <returns></returns>
            T Visit(Assembly assembly);

            /// <summary>Handle a file reference.</summary>
            /// <param name="file"></param>
            /// <returns></returns>
            T Visit(string file);

            /// <summary>Handle a stream reference.</summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            T Visit(Stream stream);

            /// <summary>Handle a byte array reference.</summary>
            /// <param name="byteArray"></param>
            /// <returns></returns>
            T Visit(byte[] byteArray);
        }

        /// <summary>The Type of the reference</summary>
        public enum CompilerReferenceType
        {
            FileReference,
            DirectAssemblyReference,
            StreamReference,
            ByteArrayReference,
        }

        /// <summary>A visitor for the GetFile function.</summary>
        private class SelectFileVisitor : ICompilerReferenceVisitor<string>
        {
            private readonly Func<string, Exception> exceptionCreator;

            public SelectFileVisitor(Func<string, Exception> exceptionCreator = null)
            {
                this.exceptionCreator = exceptionCreator ??
                                        (msg => (Exception) new InvalidOperationException(msg));
            }

            public string Visit(Assembly assembly)
            {
                var location = assembly.Location;
                if (!string.IsNullOrEmpty(location))
                    return location;
                throw this.exceptionCreator("Could not get location from assembly!");
            }

            public string Visit(string file)
            {
                return file;
            }

            public string Visit(Stream stream)
            {
                throw this.exceptionCreator("need file but got stream reference!");
            }

            public string Visit(byte[] byteArray)
            {
                throw this.exceptionCreator("need file but got byteArray reference!");
            }
        }

        /// <summary>A file reference.</summary>
        public class FileReference : CompilerReference
        {
            /// <summary>The referenced file.</summary>
            public string File { get; }

            internal FileReference(string file)
                : base(CompilerReferenceType.FileReference)
            {
                this.File = new Uri(Path.GetFullPath(file)).LocalPath;
            }

            /// <summary>Visit the given visitor.</summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="visitor"></param>
            /// <returns></returns>
            public override T Visit<T>(ICompilerReferenceVisitor<T> visitor)
            {
                return visitor.Visit(this.File);
            }
        }

        /// <summary>A direct assembly reference.</summary>
        public class DirectAssemblyReference : CompilerReference
        {
            /// <summary>The referenced assembly.</summary>
            public Assembly Assembly { get; }

            internal DirectAssemblyReference(Assembly assembly)
                : base(CompilerReferenceType.DirectAssemblyReference)
            {
                this.Assembly = assembly;
            }

            /// <summary>Visit the visitor.</summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="visitor"></param>
            /// <returns></returns>
            public override T Visit<T>(ICompilerReferenceVisitor<T> visitor)
            {
                return visitor.Visit(this.Assembly);
            }
        }

        /// <summary>A stream reference.</summary>
        public class StreamReference : CompilerReference
        {
            /// <summary>The referenced stream.</summary>
            public Stream Stream { get; }

            internal StreamReference(Stream stream)
                : base(CompilerReferenceType.StreamReference)
            {
                this.Stream = stream;
            }

            /// <summary>Visit the given visitor.</summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="visitor"></param>
            /// <returns></returns>
            public override T Visit<T>(ICompilerReferenceVisitor<T> visitor)
            {
                return visitor.Visit(this.Stream);
            }
        }

        /// <summary>A byte array reference.</summary>
        public class ByteArrayReference : CompilerReference
        {
            /// <summary>The referenced byte array.</summary>
            public byte[] ByteArray { get; }

            internal ByteArrayReference(byte[] byteArray)
                : base(CompilerReferenceType.ByteArrayReference)
            {
                this.ByteArray = byteArray;
            }

            /// <summary>Visit the given visitor.</summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="visitor"></param>
            /// <returns></returns>
            public override T Visit<T>(ICompilerReferenceVisitor<T> visitor)
            {
                return visitor.Visit(this.ByteArray);
            }
        }
    }
}
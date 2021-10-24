using System;
using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CSharp;
using JR.DevFw.Template.Compiler.Text;

namespace JR.DevFw.Template.Compiler
{
    public class CSharpProvider : IDisposable
    {
        #region Private fields

        private CSharpCodeProvider _CSharpCodePrivoder;
        private CompilerParameters _CompilerParameters = new CompilerParameters();
        private CompilerResults _Clr;
        private string _Code;
        private DateTime _LstFileTime = DateTime.Parse("1900-1-1");
        private AppDomain _AppDomain;
        private string _AssemblyFileName = "DynamicTemp.dll";
        private bool _RemoveAssemblyFile;
        private Encoding _SourceCodeFileEncoding = Encoding.Default;
        private DotNetReferences _DotNetReferences = new DotNetReferences();

        #endregion

        #region Public Properties

        /// <summary>
        /// Manage the references
        /// </summary>
        public DotNetReferences References
        {
            get { return _DotNetReferences; }
        }

        /// <summary>
        /// Get or set the output assembly file name for the dynamic source code
        /// </summary>
        public string AssemblyFileName
        {
            get { return _AssemblyFileName; }

            set { _AssemblyFileName = value; }
        }

        /// <summary>
        /// The compilier parameters
        /// </summary>
        public CompilerParameters CompilerParameters
        {
            get { return _CompilerParameters; }
        }

        /// <summary>
        /// If this property set to true,
        /// When the object intance from this class dispose,
        /// the assembly File that is compiled will be deleted.
        /// </summary>
        public bool RemoveAssemblyFile
        {
            get { return _RemoveAssemblyFile; }

            set { _RemoveAssemblyFile = value; }
        }

        /// <summary>
        /// Get or set the encoding of source code file 
        /// </summary>
        public Encoding SourceCodeFileEncoding
        {
            get { return _SourceCodeFileEncoding; }

            set { _SourceCodeFileEncoding = value; }
        }

        #endregion

        #region Private methods

        private void LoadReference(string code)
        {
            foreach (string reference in References.GetReferences())
            {
                if (reference != null)
                {
                    if (_CompilerParameters.ReferencedAssemblies.IndexOf(reference) < 0)
                    {
                        _CompilerParameters.ReferencedAssemblies.Add(reference);
                    }
                }
            }

            foreach (string nameSpace in ReferenceInCode.GetNameSpacesInSourceCode(code))
            {
                string referenceDll = References[nameSpace];
                if (referenceDll != null)
                {
                    if (_CompilerParameters.ReferencedAssemblies.IndexOf(referenceDll) < 0)
                    {
                        _CompilerParameters.ReferencedAssemblies.Add(referenceDll);
                    }
                }
            }
        }


        /// <summary>
        /// Create the instance named or inherited by typeFullName
        /// </summary>
        /// <param name="code">code string or source code file name</param>
        /// <param name="typeFullName">the full name of type</param>
        /// <param name="fromFile">from file or from code string</param>
        /// <returns>instance object</returns>
        /// <remarks>If the type named or inherited by typefullname does not exits, return null</remarks>
        private object InnerCreateInstance(string code, string typeFullName, bool fromFile)
        {
            bool reCompile = false;

            if (fromFile)
            {
                reCompile = CompileFromFile(code);
            }
            else
            {
                reCompile = Compile(code);
            }

            if (reCompile)
            {
                AppDomainSetup appDomainSetup;

                appDomainSetup = new AppDomainSetup();
                appDomainSetup.LoaderOptimization = LoaderOptimization.SingleDomain;
                appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
                appDomainSetup.ShadowCopyDirectories = appDomainSetup.ApplicationBase;
                appDomainSetup.ShadowCopyFiles = "true";
                _AppDomain = AppDomain.CreateDomain(AssemblyFileName, null, appDomainSetup);
            }

            RemoteLoader remoteLoader =
                (RemoteLoader) _AppDomain.CreateInstance("Compiler.Dynamic", "RemoteAccess.RemoteLoader").Unwrap();
            return remoteLoader.Create(AssemblyFileName, typeFullName, null);
        }

        #endregion

        #region Constructor

        public CSharpProvider()
        {
        }

        ~CSharpProvider()
        {
            try
            {
                Dispose();
            }
            catch
            {
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Compiler From source code file
        /// </summary>
        /// <param name="sourceCodeFileName">The file name of source code</param>
        /// <returns></returns>
        public bool CompileFromFile(string sourceCodeFileName)
        {
            DateTime fileTime = System.IO.File.GetLastWriteTime(sourceCodeFileName);

            if (_LstFileTime == fileTime)
            {
                return false;
            }

            _LstFileTime = fileTime;

            string code = File.ReadFileToString(sourceCodeFileName, SourceCodeFileEncoding);

            bool removeAssemblyFile = RemoveAssemblyFile;
            RemoveAssemblyFile = true;
            Dispose();
            RemoveAssemblyFile = removeAssemblyFile;

            _CSharpCodePrivoder = new CSharpCodeProvider();

            LoadReference(code);
            _Code = code;
            _CompilerParameters.GenerateInMemory = false;
            _CompilerParameters.OutputAssembly = AssemblyFileName;

            string[] files = new string[1];
            files[0] = sourceCodeFileName;

            _Clr = _CSharpCodePrivoder.CompileAssemblyFromFile(_CompilerParameters, files);

            if (_Clr.Errors.HasErrors)
            {
                throw new CompilerException(code, _Clr.Errors);
            }

            return true;
        }

        public bool Compile(string code)
        {
            if (code == _Code)
            {
                return false;
            }

            bool removeAssemblyFile = RemoveAssemblyFile;
            RemoveAssemblyFile = true;
            Dispose();
            RemoveAssemblyFile = removeAssemblyFile;

            _Code = code;
            _CSharpCodePrivoder = new CSharpCodeProvider();

            LoadReference(code);

            _CompilerParameters.GenerateInMemory = false;
            _CompilerParameters.OutputAssembly = AssemblyFileName;

            _Clr = _CSharpCodePrivoder.CompileAssemblyFromSource(_CompilerParameters, code);

            if (_Clr.Errors.HasErrors)
            {
                throw new CompilerException(code, _Clr.Errors);
            }

            return true;
        }


        /// <summary>
        /// Create the instance named or inherited by typeFullName
        /// </summary>
        /// <param name="code">source code string</param>
        /// <param name="typeFullName">the full name of type</param>
        /// <returns>instance object</returns>
        /// <remarks>If the type named or inherited by typefullname does not exits, return null</remarks>
        public object CreateInstance(string code, string typeFullName)
        {
            return InnerCreateInstance(code, typeFullName, false);
        }

        /// <summary>
        /// Create the instance named or inherited by typeFullName
        /// </summary>
        /// <param name="fileName">source code file</param>
        /// <param name="typeFullName">the full name of type</param>
        /// <returns>instance object</returns>
        /// <remarks>If the type named or inherited by typefullname does not exits, return null</remarks>
        public object CreateInstanceFromFile(string fileName, string typeFullName)
        {
            return InnerCreateInstance(fileName, typeFullName, true);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_AppDomain != null)
            {
                AppDomain.Unload(_AppDomain);
                _AppDomain = null;
            }

            _Clr = null;

            GC.Collect();

            if (RemoveAssemblyFile)
            {
                if (System.IO.File.Exists(AssemblyFileName))
                {
                    System.IO.File.Delete(AssemblyFileName);
                }

                if (System.IO.File.Exists(System.IO.Path.GetFileName(AssemblyFileName) + ".pdb"))
                {
                    System.IO.File.Delete(System.IO.Path.GetFileName(AssemblyFileName) + ".pdb");
                }
            }
        }

        #endregion
    }
}
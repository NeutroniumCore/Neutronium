using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Navigation;

namespace MVVM.HTML.Core
{
    public class NavigationBuilder : INavigationBuilder, IUrlSolver
    {
        private IDictionary<Type, IDictionary<string, Uri>> _Mapper = new Dictionary<Type, IDictionary<string, Uri>>();

        private void Register(Type itype, Uri uri, string id)
        {
            try
            {
                IDictionary<string, Uri> res = _Mapper.FindOrCreateEntity(itype, t => new Dictionary<string, Uri>());
                res.Add(id ?? string.Empty, uri);
            }
            catch (ArgumentException)
            {
                throw ExceptionHelper.GetArgument(string.Format("A same ViewModel type can not be registered twice {0}", itype));
            }
        }

        private void CheckPath(string iPath)
        {
            if (!File.Exists(iPath))
                throw ExceptionHelper.GetArgument(string.Format("Registered path does not exist: {0}", iPath));
        }

        private Uri CreateUri(string iPath)
        {
            CheckPath(iPath);
            return new Uri(iPath);
        }


        public void Register<T>(string iPath, string Id = null)
        {
            Register(typeof(T), CreateUri(string.Format("{0}\\{1}", Assembly.GetCallingAssembly().GetPath(), iPath)), Id);
        }

        public void RegisterAbsolute<T>(string iPath, string Id = null)
        {
            Register(typeof(T), CreateUri(iPath), Id);
        }

        public void Register<T>(Uri iPath, string Id = null)
        {
            CheckPath(iPath.LocalPath);
            Register(typeof(T), iPath, Id);
        }

        private Uri SolveType(Type iType, string id)
        {
            IDictionary<string, Uri> dicres = null;
            Uri res = null;

            foreach (Type InType in iType.GetBaseTypes())
            {
                if (_Mapper.TryGetValue(InType, out dicres))
                {
                    if (dicres.TryGetValue(id, out res))
                        return res;
                }
            }
            return null;
        }

        public Uri Solve(object iViewModel, string Id = null)
        {
            string id = Id??string.Empty;
            Uri res = SolveType(iViewModel.GetType(), id);
            if (res != null)
                return res;

            if (id!=string.Empty)
                return SolveType(iViewModel.GetType(), string.Empty);

            return null;
        }
    }
}

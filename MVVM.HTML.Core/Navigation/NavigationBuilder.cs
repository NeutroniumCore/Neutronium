using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;

namespace MVVM.HTML.Core.Navigation
{
    public class NavigationBuilder : INavigationBuilder, IUrlSolver
    {
        private IDictionary<Type, IDictionary<string, Uri>> _Mapper = new Dictionary<Type, IDictionary<string, Uri>>();

        private void Register(Type type, Uri uri, string id)
        {
            try
            {
                IDictionary<string, Uri> res = _Mapper.FindOrCreateEntity(type, t => new Dictionary<string, Uri>());
                res.Add(id ?? string.Empty, uri);
            }
            catch (ArgumentException)
            {
                throw ExceptionHelper.GetArgument(string.Format("A same ViewModel type can not be registered twice {0}", type));
            }
        }

        private void CheckPath(string path)
        {
            if (!File.Exists(path))
                throw ExceptionHelper.GetArgument(string.Format("Registered path does not exist: {0}", path));
        }

        private Uri CreateUri(string path)
        {
            CheckPath(path);
            return new Uri(path);
        }

        public void Register<T>(string path, string id = null)
        {
            Register(typeof(T), CreateUri(string.Format("{0}\\{1}", Assembly.GetCallingAssembly().GetPath(), path)), id);
        }

        public void RegisterAbsolute<T>(string path, string id = null)
        {
            Register(typeof(T), CreateUri(path), id);
        }

        public void Register<T>(Uri path, string id = null)
        {
            CheckPath(path.LocalPath);
            Register(typeof(T), path, id);
        }

        private Uri SolveType(Type type, string id)
        {
            IDictionary<string, Uri> dicres = null;
            Uri res = null;

            foreach (Type InType in type.GetBaseTypes())
            {
                if (_Mapper.TryGetValue(InType, out dicres))
                {
                    if (dicres.TryGetValue(id, out res))
                        return res;
                }
            }
            return null;
        }

        public Uri Solve(object viewModel, string iId = null)
        {
            string id = iId ?? string.Empty;
            Uri res = SolveType(viewModel.GetType(), id);
            if (res != null)
                return res;

            if (id!=string.Empty)
                return SolveType(viewModel.GetType(), string.Empty);

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using MoreCollection.Extensions;

namespace Neutronium.Core.Navigation
{
    public class NavigationBuilder : INavigationBuilder, IUrlSolver
    {
        private IDictionary<Type, IDictionary<string, Uri>> _Mapper = new Dictionary<Type, IDictionary<string, Uri>>();

        private void Register(Type type, Uri uri, string id)
        {
            try
            {
                var res = _Mapper.GetOrAddEntity(type, t => new Dictionary<string, Uri>());
                res.Add(id ?? string.Empty, uri);
            }
            catch (ArgumentException)
            {
                throw ExceptionHelper.GetArgument($"A same ViewModel type can not be registered twice {type}");
            }
        }

        private void CheckPath(string path)
        {
            if (!File.Exists(path))
                throw ExceptionHelper.GetArgument($"Registered path does not exist: {path}");
        }

        private Uri CreateUri(string path)
        {
            CheckPath(path);
            return new Uri(path);
        }

        public void Register<T>(string path, string id = null)
        {
            Register(typeof(T), CreateUri($"{Assembly.GetCallingAssembly().GetPath()}\\{path}"), id);
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
            foreach (var inType in type.GetBaseTypes())
            {
                IDictionary<string, Uri> dicres;
                if (!_Mapper.TryGetValue(inType, out dicres))
                    continue;

                Uri res;
                if (dicres.TryGetValue(id, out res))
                    return res;
            }
            return null;
        }

        public Uri Solve(object viewModel, string id = "")
        {
            var type = viewModel.GetType();
            var res = SolveType(type, id);
            if (res != null)
                return res;

            return (!string.IsNullOrEmpty(id)) ? SolveType(type, string.Empty) : null;
        }
    }
}

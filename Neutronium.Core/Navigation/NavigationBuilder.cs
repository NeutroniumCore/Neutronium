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
        private readonly IDictionary<Type, IDictionary<string, Uri>> _Mapper = new Dictionary<Type, IDictionary<string, Uri>>();

        private NavigationBuilder Register(Type type, Uri uri, string id)
        {
            try
            {
                var res = _Mapper.GetOrAddEntity(type, t => new Dictionary<string, Uri>());
                res.Add(id ?? string.Empty, uri);
                return this;
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

        private Uri CreateUriFromLocal(string path)
        {
            return CreateUri($"{Assembly.GetCallingAssembly().GetPath()}\\{path}");
        }

        public INavigationBuilder Register(Type type, string path, string id = null)
        {
            return Register(type, CreateUriFromLocal(path), id);
        }

        public INavigationBuilder Register<T>(string path, string id = null)
        {
            return Register(typeof(T), CreateUriFromLocal(path), id);
        }

        public INavigationBuilder RegisterAbsolute<T>(string path, string id = null)
        {
            return Register(typeof(T), CreateUri(path), id);
        }

        public INavigationBuilder Register<T>(Uri path, string id = null)
        {
            CheckPath(path.LocalPath);
            return Register(typeof(T), path, id);
        }

        private Uri SolveType(Type type, string id)
        {
            id = id ?? string.Empty;
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

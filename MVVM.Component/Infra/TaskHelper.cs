using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Component.Infra
{
    public static class TaskHelper
    {
        public static async Task<object> Convert<T>(this Task<T> task)
        {
            T res = await task;
            return res;
        }
    }
}

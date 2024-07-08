using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Abs
{
    public interface IRolesService <T>
    {
        Task<TaskResult<List<T>>> GetAll ();
        Task<TaskResult<T>> Get (string id);
        Task<TaskResult<T>> Create (T model);
        Task<TaskResult<T>> Delete (string id);
        Task<TaskResult<T>> Update (T model);
    }
}

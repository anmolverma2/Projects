
using MediatR;
using MicroServicesProject.Infrastructure;
using MicroServicesProject.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MicroServicesProject.Commands
{
    public class GetStudentsQuery : IRequest<ResponseModel>
    {
        public string? Class { get; set; }
        public string? Name { get; set; }  

        protected class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, ResponseModel>
        {
            private readonly IConnectionFactory _dal;
            public GetStudentsQueryHandler(IConnectionFactory dAL)
            {
                _dal = dAL;
            }
            public async Task<ResponseModel> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
            {
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@className",SqlDbType.NVarChar){Value = request.Class},
                    new SqlParameter("@Name",SqlDbType.NVarChar){Value = request.Name},
                    new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "List"},
                };

                try
                {
                    var data = _dal.GetDAL.SPExecuteToList<StudentModel>("SP_Students", sqlParameters, "DS");

                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "Success",
                        Data = data
                    };
                }
                catch (Exception ex) {
                    return new ResponseModel
                    {
                        Code = 1,
                        Message = "Error",
                        Data = ex.Message.ToString()
                    };

                }
            }
        }

    }
}

using MediatR;
using MicroServicesProject.Infrastructure;
using MicroServicesProject.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace MicroServicesProject.Commands
{
    public class GetAllDetailsQuery : IRequest<ResponseModel>
    {
        public string? Name { get; set; }
        protected class GetAllDetailsQueryHandler : IRequestHandler<GetAllDetailsQuery, ResponseModel>
        {
            private readonly IConnectionFactory _con;
            public GetAllDetailsQueryHandler(IConnectionFactory con)
            {
                _con = con;
            }
            public async Task<ResponseModel> Handle(GetAllDetailsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    SqlParameter[] sqlParameters = {
                        new SqlParameter("@Name",SqlDbType.NVarChar){Value=request.Name},
                        new SqlParameter("@operationtype",SqlDbType.NVarChar){Value = "List"}
                    };

                    var data = _con.GetDAL.SPExecuteToList<DetailsModel>("[dbo].[SP_Details]", sqlParameters, "DS");

                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Success",
                        Data = data
                    };
                }
                catch (Exception ex) {

                    return new ResponseModel
                    {
                        Code = 0,
                        Message = "Error in data retrieval",
                        Data = ex.Message.ToString()
                    };
                }
            }
        }
    }
}

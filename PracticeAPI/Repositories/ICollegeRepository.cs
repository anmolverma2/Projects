using PracticeAPI.Common;
using PracticeAPI.Model;

namespace PracticeAPI.Repositories
{
    public interface ICollegeRepository : ICommonRepository<CollegeStudent>
    {
        Task<List<CollegeStudent>> GetFilterData();
         //Task<List<CollegeStudent>> GetAllAsync();
         //Task<CollegeStudent> GetById(int id);
         //Task<CollegeStudent> GetByName(string name);
         //Task<bool> DeleteStudent(CollegeStudent student);
         //Task<int> Update(StudentDTO student);
         //Task<int> Create(CollegeStudent student);

    }
}

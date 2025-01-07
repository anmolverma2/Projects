using Microsoft.EntityFrameworkCore;
using PracticeAPI.Common;
using PracticeAPI.Context;
using PracticeAPI.Model;

namespace PracticeAPI.Repositories
{
    public class CollegeRepository : CommonRepository<CollegeStudent>,ICollegeRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public CollegeRepository(ApplicationDBContext dBContext) : base(dBContext) 
        {
            _dbContext = dBContext;
        }

        public async Task<List<CollegeStudent>> GetFilterData()
        {
            return null;
        }

        //public async Task<List<CollegeStudent>> GetAllAsync()
        //{
        //    return await _dbContext.collegeStudents.ToListAsync();
        //}

        //public async Task<CollegeStudent> GetById(int id)
        //{
        //    return await _dbContext.collegeStudents.Where(n => n.Id == id).FirstOrDefaultAsync();
        //}

        //public async Task<CollegeStudent> GetByName(string name)
        //{
        //    return await _dbContext.collegeStudents.Where(n => n.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        //}
        //public async Task<int> Create(CollegeStudent student)
        //{
        //     await _dbContext.collegeStudents.AddAsync(student);
        //     await _dbContext.SaveChangesAsync();
        //    return student.Id;

        //}

        //public async Task<bool> DeleteStudent(CollegeStudent student)
        //{
        //    var record = await _dbContext.collegeStudents.Where(n => n.Id == student.Id).FirstOrDefaultAsync();
        //    _dbContext.collegeStudents.Remove(record);
        //    await  _dbContext.SaveChangesAsync();
        //    return true;
        //}
        //public async Task<int> Update(StudentDTO student)
        //{
        //    var studentToUpdate = await _dbContext.collegeStudents.Where(n => n.Id == student.Id).FirstOrDefaultAsync();
        //    if (studentToUpdate == null)
        //        throw new ArgumentNullException($"No record found with the student id {student.Id}");

        //    studentToUpdate.Name = student.Name;
        //    studentToUpdate.Email = student.Email;
        //    studentToUpdate.Age = student.Age;
        //    studentToUpdate.Address = student.Address;
        //    studentToUpdate.DOB = student.DOB;
        //    studentToUpdate.Mobile = student.Mobile;

        //    await _dbContext.SaveChangesAsync();

        //    return student.Id;
        //}
    }
}

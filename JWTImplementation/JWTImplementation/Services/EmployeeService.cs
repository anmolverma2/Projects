using JWTImplementation.Context;
using JWTImplementation.Interface;
using JWTImplementation.Models;

namespace JWTImplementation.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly JWTContext _jwtContext;
        public EmployeeService(JWTContext context)
        {
            _jwtContext = context;
        }
        public Employee AddEmployee(Employee employee)
        {
            var emp = _jwtContext.Employees.Add(employee);
            _jwtContext.SaveChanges();
            return emp.Entity;
        }

        public bool DeleteEmployee(int id)
        {
            try
            {
                var emp = _jwtContext.Employees.SingleOrDefault(x => x.Id == id);
                if (emp != null)
                {
                    _jwtContext.Remove(emp);
                    _jwtContext.SaveChanges();
                    return true;
                }
                else
                {
                     throw new Exception("User not found !");

                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Employee GetEmployeeDetail(int id)
        {
            var employee = _jwtContext.Employees.SingleOrDefault(s => s.Id == id);
            return employee;
        }

        public List<Employee> GetEmployeeDetails()
        {
            var employees = _jwtContext.Employees.ToList();
            return employees;
        }

        public Employee UpdateEmployee(Employee employee)
        {
            var updated = _jwtContext.Employees.Update(employee);
            _jwtContext.SaveChanges();
            return updated.Entity;
        }
    }
}

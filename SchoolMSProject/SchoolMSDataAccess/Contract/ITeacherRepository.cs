﻿using SchoolMSDataAccess.Entities.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolMSDataAccess.Contract
{
    public interface ITeacherRepository
    {
        int InsertTeacherData(TeacherModel model);

    }
}

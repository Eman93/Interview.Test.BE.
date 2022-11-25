using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraduationTracker
{
    public partial class GraduationTracker
    {   
        public Tuple<bool, STANDING>  HasGraduated(Diploma diploma, Student student)
        {
            int studentCredits = 0, studentTotalMarks = 0, numberOfCoursesTaken=0 ;

            // get diploma requirements 
            var requirements = Repository.GetRequirements().Where(req => diploma.Requirements.Any(reqId => reqId == req.Id));

            foreach (var req in requirements)
            {
                /*
                 I think "Requirement" object should have one to one relationship with "Course"
                 not one to many

                 so the code would have been as follow,and wouldn't use another foreach

                  int courseId = req.Courses.SingleOrDefault();           

                 */

                //check student mark for courses in the requirment

                bool hasPassedAllCourses = true;
                foreach (var courseId in req.Courses)
                {
                    var studentCourse = student.Courses.Where(course => course.Id == courseId).SingleOrDefault();

                    if (studentCourse!=null && studentCourse.Mark >= req.MinimumMark)
                    {
                        studentTotalMarks += studentCourse.Mark;
                        numberOfCoursesTaken++;

                    }
                    else { hasPassedAllCourses = false; }
                }

                studentCredits += hasPassedAllCourses ? req.Credits : 0;

            }


            // if student criedts are not enough, no need to calculate the standing
            if(studentCredits < diploma.Credits)
            {
                return new Tuple<bool, STANDING>(false, STANDING.None);
            }

            /*
             * I am not sure how we should calculate the standing or the gpa in general and
             * if we need the course full mark or no (it is not given).
             * So I will leave the calculation logic as it is for now with changes in the code style itself
             * 
             */


            bool hasGraduated = true;
            int average = studentTotalMarks / numberOfCoursesTaken;

            var standing = STANDING.None;

            if (average < 50)
            {
                standing = STANDING.Remedial;
                hasGraduated = false;
            }
            else if (average < 80)
                standing = STANDING.Average;
            else if (average < 95)
                standing = STANDING.MagnaCumLaude;
            else
                standing = STANDING.SumaCumLaude;

            return new Tuple<bool, STANDING>(hasGraduated, standing);

        }
    }
}

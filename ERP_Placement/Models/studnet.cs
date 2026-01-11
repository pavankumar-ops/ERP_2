namespace ERP_Placement.Models
{
    public class StudentRegistration
    {
        // -------------------- StudentDetails Table --------------------

        public string StudentId { get; set; }
        public string Name_Title { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string Gender { get; set; }
        public DateTime DOB { get; set; }

        public string Email { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobile { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Personal_Photo { get; set; }




        public string Acadamic_Year { get; set; }
        public string Roll_No { get; set; }
        public string Branch { get; set; }
        public string Divison { get; set; }
        public string Password { get; set; }






        // -------------------- StudentEducation Table --------------------

        // SSC
        public string SSC { get; set; }
        public string SSC_Passing_Year { get; set; }
        public string SSC_Document_Path { get; set; }

        // HSC
        public string HSC { get; set; }
        public string HSC_Passing_Year { get; set; }
        public string HSC_Document_Path { get; set; }

        // Diploma
        public string Diploma_Branch { get; set; }
        public string Diploma_Percentages { get; set; }
        public string Diploma_Passing_Year { get; set; }
        public string Diploma_Document_Path { get; set; }

        // BTech
        public string BTech_Branch { get; set; }
        public string BTech_Passing_Year { get; set; }
        public string BTech_Percentages { get; set; }
        public string BTech_Document_Path { get; set; }

        public string Skills { get; set; }
        public string ResumePath { get; set; }

        // Upload properties (not DB)
        public IFormFile SSC_File { get; set; }
        public IFormFile HSC_File { get; set; }
        public IFormFile Diploma_File { get; set; }
        public IFormFile BTech_File { get; set; }
        public IFormFile Resume { get; set; }
        public IFormFile Profile_photo { get; set; }
    }

}

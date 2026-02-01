namespace ERP_Placement.Models
{
    public class Placement_Coordinator_Model
    {
        public string StudentId { get; set; }
        
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



        //comapny registration
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyDomain { get; set; }
        public string HRName{ get; set; }
        public string HRMail { get; set; }
        public string HRMobNo { get; set; }

        public string RegisteredBy { get; set; }
        public DateTime CompanyRegistrationDate { get; set; }

        // job vaccacy

        public string lastDate { get; set; }
        public string salary { get; set; }
        public string location { get; set; }
        public string openings { get; set; }
        public string jobTitle { get; set; }

    }
}

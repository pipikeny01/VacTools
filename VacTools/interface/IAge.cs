using System;

namespace VacWebSiteTools
{
    public interface IAge
    {
        int Years { set; get; }
        int Months { set; get; }
        int Days { set; get; }
        int CalculateAge(DateTime birthDate, DateTime endDate);
    }
}
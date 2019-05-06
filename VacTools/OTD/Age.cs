using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class Age
    {
        public int Years { set; get; }
        public int Months { set; get; }
        public int Days { set; get; }

        public Age CalculateAge(DateTime birthDate, DateTime endDate)
        {
            if (birthDate.Date > endDate.Date)
                throw new ArgumentException("birthDate cannot be higher then endDate", "birthDate");

            int years = endDate.Year - birthDate.Year;
            int months = 0;
            int days = 0;

            // Check if the last year, was a full year.
            if (endDate < birthDate.AddYears(years) && years != 0)
                years--;

            // Calculate the number of months.
            birthDate = birthDate.AddYears(years);

            if (birthDate.Year == endDate.Year)
                months = endDate.Month - birthDate.Month;
            else
                months = (12 - birthDate.Month) + endDate.Month;

            // Check if last month was a complete month.
            if (endDate < birthDate.AddMonths(months) && months != 0)
                months--;

            // Calculate the number of days.
            birthDate = birthDate.AddMonths(months);

            days = (endDate - birthDate).Days;

            return new Age
            {
                Years = years,
                Months = months,
                Days = days
            };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JR.Stand.Core.Framework.Scheduler
{
    public interface ICronSchedule
    {
        bool IsValid(string expression);
        bool IsTime(DateTime dateTime);
    }

    public class CronSchedule : ICronSchedule
    {
        #region Readonly Class Members

        private static readonly Regex DividedRegex = new Regex(@"(\*/\d+)");
        private static readonly Regex RangeRegex = new Regex(@"(\d+\-\d+)\/?(\d+)?");
        private static readonly Regex WildRegex = new Regex(@"(\*)");
        private static readonly Regex ListRegex = new Regex(@"(((\d+,)*\d+)+)");
        private static readonly Regex ValidationRegex = new Regex(DividedRegex + "|" + RangeRegex + "|" + WildRegex + "|" + ListRegex);

        #endregion

        #region Private Instance Members

        private readonly string _expression;
        private List<int> _minutes;
        private List<int> _hours;
        private List<int> _daysOfMonth;
        private List<int> _months;
        private List<int> _daysOfWeek;

        #endregion

        #region Public Constructors

        public CronSchedule()
        {
        }

        public CronSchedule(string expressions)
        {
            this._expression = expressions;
            Generate();
        }

        #endregion

        #region Public Methods

        private bool IsValid()
        {
            return IsValid(this._expression);
        }

        public bool IsValid(string expression)
        {
            MatchCollection matches = ValidationRegex.Matches(expression);
            return matches.Count > 0;//== 5;
        }

        public bool IsTime(DateTime dateTime)
        {
            return _minutes.Contains(dateTime.Minute) &&
                   _hours.Contains(dateTime.Hour) &&
                   _daysOfMonth.Contains(dateTime.Day) &&
                   _months.Contains(dateTime.Month) &&
                   _daysOfWeek.Contains((int)dateTime.DayOfWeek);
        }

        private void Generate()
        {
            if (!IsValid()) return;

            MatchCollection matches = ValidationRegex.Matches(this._expression);

            generate_minutes(matches[0].ToString());

            generate_hours(matches.Count > 1 ? matches[1].ToString() : "*");

            generate_days_of_month(matches.Count > 2 ? matches[2].ToString() : "*");

            generate_months(matches.Count > 3 ? matches[3].ToString() : "*");
            generate_days_of_weeks(matches.Count > 4 ? matches[4].ToString() : "*");
        }

        private void generate_minutes(string match)
        {
            this._minutes = generate_values(match, 0, 60);
        }

        private void generate_hours(string match)
        {
            this._hours = generate_values(match, 0, 24);
        }

        private void generate_days_of_month(string match)
        {
            this._daysOfMonth = generate_values(match, 1, 32);
        }

        private void generate_months(string match)
        {
            this._months = generate_values(match, 1, 13);
        }

        private void generate_days_of_weeks(string match)
        {
            this._daysOfWeek = generate_values(match, 0, 7);
        }

        private List<int> generate_values(string configuration, int start, int max)
        {
            if (DividedRegex.IsMatch(configuration)) return divided_array(configuration, start, max);
            if (RangeRegex.IsMatch(configuration)) return rangeArray(configuration);
            if (WildRegex.IsMatch(configuration)) return wildArray(configuration, start, max);
            if (ListRegex.IsMatch(configuration)) return listArray(configuration);

            return new List<int>();
        }

        private List<int> divided_array(string configuration, int start, int max)
        {
            if (!DividedRegex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();
            string[] split = configuration.Split("/".ToCharArray());
            int divisor = int.Parse(split[1]);

            for (int i = start; i < max; ++i)
                if (i % divisor == 0)
                    ret.Add(i);

            return ret;
        }

        private List<int> rangeArray(string configuration)
        {
            if (!RangeRegex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();
            string[] split = configuration.Split("-".ToCharArray());
            int start = int.Parse(split[0]);
            int end = 0;
            if (split[1].Contains("/"))
            {
                split = split[1].Split("/".ToCharArray());
                end = int.Parse(split[0]);
                int divisor = int.Parse(split[1]);

                for (int i = start; i < end; ++i)
                    if (i % divisor == 0)
                        ret.Add(i);
                return ret;
            }
            else
                end = int.Parse(split[1]);

            for (int i = start; i <= end; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> wildArray(string configuration, int start, int max)
        {
            if (!WildRegex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();

            for (int i = start; i < max; ++i)
                ret.Add(i);

            return ret;
        }

        private List<int> listArray(string configuration)
        {
            if (!ListRegex.IsMatch(configuration))
                return new List<int>();

            List<int> ret = new List<int>();

            string[] split = configuration.Split(",".ToCharArray());

            foreach (string s in split)
                ret.Add(int.Parse(s));

            return ret;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public static class MatchesFriends
    {
        public static bool IsInAgeRange(User i_UserLoggedIn, User i_Friend, int i_NumberOfYears, bool i_IsBig)
        {
            DateTime CurrentUserBirthDate = Convert.ToDateTime(i_UserLoggedIn.Birthday);
            DateTime friendsBirthDate = Convert.ToDateTime(i_Friend.Birthday);
            bool isInRange = false;

            int selectedRange = CurrentUserBirthDate.Year - i_NumberOfYears;
            if (i_IsBig)
            {
                if (friendsBirthDate.Year >= selectedRange && friendsBirthDate.Year <= CurrentUserBirthDate.Year)
                {
                    isInRange = true;
                }
               
            }
            else
            {
                if (friendsBirthDate.Year >= CurrentUserBirthDate.Year && friendsBirthDate.Year <= selectedRange)
                {
                    isInRange = true;
                }
            }

            return isInRange;
        }
        public static bool IsGenderEqual(User i_User, string i_Gender)
        {
            bool isSameGender = false;
            string userGender = Enum.GetName(typeof(User.eGender), i_User.Gender);
            if(i_User.Gender.ToString().Equals(i_Gender))
            {
                isSameGender = true;
            }
            return isSameGender;
        }
        public static bool IslocationEqual(User i_CurrentUser, User i_Friend)
        {
            bool isSameLocation = false;
           
            if (i_CurrentUser.Location.Name.Equals(i_Friend.Location.Name))
            {
                isSameLocation = true;
            }
            return isSameLocation;
           
        }
    }
}

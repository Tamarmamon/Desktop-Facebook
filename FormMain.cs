using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FacebookWrapper.ObjectModel;
using FacebookWrapper;

namespace BasicFacebookFeatures
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            FacebookWrapper.FacebookService.s_CollectionLimit = 100;
        }

        User m_LoggedInUser;
        LoginResult m_LoginResult;
        public FacebookObjectCollection<Album> m_albumsListToShow = new FacebookObjectCollection<Album>();
        public int m_IntImgNum = 0;

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            
            m_LoginResult = FacebookService.Login("926372037976387",
                   "email",
                   "public_profile",
                   "user_age_range",
                   "user_birthday",
                   "user_events",
                   "user_friends",
                   "user_gender",
                   "user_hometown",
                   "user_likes",
                   "user_link",
                   "user_location",
                   "user_photos",
                   "user_posts",
                   "user_videos");

            if (!string.IsNullOrEmpty(m_LoginResult.AccessToken))
            {
                m_LoggedInUser = m_LoginResult.LoggedInUser;

                fetchUserInfo();
                showFriendsList();
                showAlbumsList();
                showPostsList();
                showTaggedPhotos();
            }
            else
            {
                MessageBox.Show(m_LoginResult.ErrorMessage, "Login Failed");
            }

        }
        private void fetchUserInfo()
        {
            profilPicture.Image = m_LoggedInUser.ImageLarge;
            firstName.Text = m_LoggedInUser.FirstName;
            lastName.Text = m_LoggedInUser.LastName;
            Birthday.Text = m_LoggedInUser.Birthday;
            Gender.Text = m_LoggedInUser.Gender.ToString();
            relationshipStatus.Text = m_LoggedInUser.RelationshipStatus.ToString();
  
        }
        private void showFriendsList()
        {
            if (m_LoggedInUser.Friends.Count == 0)
            {
                listBoxFriends.Items.Add("No friends");
            }
            else
            {
                foreach (User user in m_LoggedInUser.Friends)
                {
                    listBoxFriends.Items.Add(user.Name);
                }
            }         
        }

        private void showAlbumsList()
        {

            if (m_LoggedInUser.Albums.Count == 0)
            {
                checkedListBoxAlbums.Items.Add("No Albums to show");
            }
            else
            {
                foreach (Album album in m_LoggedInUser.Albums)
                {
                    //listBoxAlbums.Items.Add(album.Name);
                    checkedListBoxAlbums.Items.Add(album, CheckState.Unchecked);
                }
            }
        }

        private void showPostsList()
        {
            listBoxPosts.Items.Clear();

            foreach (Post post in m_LoggedInUser.Posts)
            {
                if (post.Message != null)
                {
                    listBoxPosts.Items.Add(post.Message);
                }
                else if (post.Caption != null)
                {
                    listBoxPosts.Items.Add(post.Caption);
                }
                else
                {
                    listBoxPosts.Items.Add(string.Format("[{0}]", post.Type));
                }
            }

            if (listBoxPosts.Items.Count == 0)
            {
                listBoxPosts.Items.Add("No posts");
            }

        }

        private void showTaggedPhotos()
        {
            
            if (m_LoggedInUser.PhotosTaggedIn.Count == 0)
            {
                listOfTaggedPhotos.Items.Add("No tagged photos to show");
            }
            else
            {
                ImageList imagesList = PhotoUtilsLogic.ListOfPhotosGenerator(m_LoggedInUser.PhotosTaggedIn);

                listOfTaggedPhotos.SmallImageList = imagesList;
                for (int i = 0; i < imagesList.Images.Count; i++)
                {
                    listOfTaggedPhotos.Items.Add("", i);
                }
            }
            
        }
        
        private void buttonLogout_Click(object sender, EventArgs e)
        {
			FacebookService.LogoutWithUI();
			buttonLogin.Text = "Login";
            
		}

        private void saveButton_Click(object sender, EventArgs e)
        {
            m_albumsListToShow.Clear();

            listViewPhotosFeature1.Items.Clear();
            foreach (Album checkedAlbum in checkedListBoxAlbums.CheckedItems)
            {
                m_albumsListToShow.Add(checkedAlbum);

            }

            ImageList imagesList = PhotoUtilsLogic.ListOfPhotosGeneratorFromAlbums(m_albumsListToShow);

            listViewPhotosFeature1.SmallImageList = imagesList;
            for (int i = 0; i < imagesList.Images.Count; i++)
            {
                listViewPhotosFeature1.Items.Add("", i);
            }

            //clear all checked albums
            foreach (int i in checkedListBoxAlbums.CheckedIndices)
            {
                checkedListBoxAlbums.SetItemCheckState(i, CheckState.Unchecked);
            }
        }

        private void slideshowButton_Click(object sender, EventArgs e)
        {

            ImageList images = PhotoUtilsLogic.ListOfPhotosGeneratorFromAlbums(m_albumsListToShow);
            imageListAlbums = images;

            timerSlideshow.Enabled = true;
        }

        private void timerSlideshow_Tick(object sender, EventArgs e)
        {
            if (imageListAlbums.Images.Count != 0)
            {
                slideshowAlbums.Image = imageListAlbums.Images[m_IntImgNum];
                if (m_IntImgNum == imageListAlbums.Images.Count - 1)
                {
                    m_IntImgNum = 0;
                }
                else
                {
                    m_IntImgNum++;
                }
            }
            else
            {
                timerSlideshow.Enabled = false;
                MessageBox.Show("Please choose albums to show");
            }
        }

        private void buttonSearchMatches_Click(object sender, EventArgs e)
        {
            listBoxPresentResults.Items.Clear();

            string chosenGender;
            if (radioButtonFemale.Checked)
            {
                chosenGender = "female";
            }
            else
            {
                chosenGender = "male";
            }

            bool isCloseBy = checkBoxSameLocation.Checked;

            int ageRange = trackBarAgeRange.Value;
            bool isBig = ageRange > 0;

            FacebookObjectCollection<User> matchedFriendsResult = new FacebookObjectCollection<User>();
            matchedFriendsResult.Clear();

            foreach (User user in m_LoggedInUser.Friends)
            {
                if (MatchesFriends.IsGenderEqual(user, chosenGender))
                {
                    if (MatchesFriends.IsInAgeRange(m_LoggedInUser, user, ageRange, isBig))
                    {
                        if (isCloseBy && MatchesFriends.IslocationEqual(m_LoggedInUser, user))
                        {
                            matchedFriendsResult.Add(user);
                        }
                        else if (!isCloseBy && !MatchesFriends.IslocationEqual(m_LoggedInUser, user))
                        {
                            matchedFriendsResult.Add(user);
                        }
                    }
                }
            }
            if (matchedFriendsResult.Count == 0)
            {
                listBoxPresentResults.Items.Add("No matches");
            }
            else
            {
                foreach (User user in matchedFriendsResult)
                {

                    listBoxPresentResults.Items.Add(user.Name);
                }
            }
            textBoxLocation.Text = m_LoggedInUser.Location.Name;
        }

        private void profilPicture_Click(object sender, EventArgs e)
        {

        }

        private void Birthday_Click(object sender, EventArgs e)
        {

        }

        private void relationshipStatus_Click(object sender, EventArgs e)
        {

        }
      
        private void friendsListTag_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void firstName_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lastName_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Gender_Click(object sender, EventArgs e)
        {

        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void slideshowAlbums_Click(object sender, EventArgs e)
        {
           
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }    
    }
}

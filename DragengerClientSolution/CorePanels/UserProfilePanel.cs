using EntityLibrary;
using ResourceLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorePanels
{
    internal class UserProfilePanel : Panel
    {
        private Label profileIconLabel, nameLabel, usernameLabel, genderLabel, birthdateLabel, friendSinceLabel;
        private Label blockFriendButton, unfriendButton;

        private Consumer consumer;
        private Panel userParentPanel;

        public UserProfilePanel(Consumer consumer, Panel userParentPanel)
        {
            this.consumer = consumer;
            this.userParentPanel = userParentPanel;
            this.BackColor = Color.FromArgb(51,51,51);
            this.Click += (s, e) => { this.Visible = false; };
            this.InitializeProfileInfo();
        }

        private void InitializeProfileInfo()
        {
            profileIconLabel = new Label();
            profileIconLabel.Image = new Bitmap(this.consumer.ProfileImage, new Size(50, 50));
            profileIconLabel.Size = profileIconLabel.Image.Size;
            this.Controls.Add(profileIconLabel);
            profileIconLabel.Click += (s, e) => { this.Visible = false; };

            nameLabel = new Label();
			nameLabel.Text = consumer.Name;
            nameLabel.Font = CustomFonts.SmallerBold;
            nameLabel.Size = nameLabel.PreferredSize;
            nameLabel.ForeColor = Color.FromArgb(234, 234, 234);
            this.Controls.Add(nameLabel);

            usernameLabel = new Label();
			usernameLabel.Text = consumer.Username;
            usernameLabel.Font = CustomFonts.New(CustomFonts.SmallerSize, 'i');
            usernameLabel.Size = usernameLabel.PreferredSize;
            usernameLabel.ForeColor = Color.FromArgb(234, 234, 234);
            this.Controls.Add(usernameLabel);

            int maxLabelWidth = Math.Max(nameLabel.PreferredWidth, usernameLabel.PreferredWidth);
            this.Width = maxLabelWidth + (maxLabelWidth / 3);

            blockFriendButton = new Label();
            blockFriendButton.Text = "🛇";
            blockFriendButton.TextAlign = ContentAlignment.MiddleCenter;
            blockFriendButton.BackColor = this.BackColor;
            blockFriendButton.ForeColor = Color.FromArgb(234, 234, 234);
            blockFriendButton.Font = CustomFonts.Smaller;
            blockFriendButton.Size = blockFriendButton.PreferredSize;
            blockFriendButton.MouseEnter += (s, e) => { blockFriendButton.BackColor = Color.FromArgb(160, 0, 0); };
            blockFriendButton.MouseLeave += (s, e) => { blockFriendButton.BackColor = this.BackColor; };
            this.Controls.Add(blockFriendButton);

            unfriendButton = new Label();
            this.CheckifyFriend();
            unfriendButton.Click += (s, e) => 
            {
                this.UnfriendifyFriend();
            };
            unfriendButton.TextAlign = ContentAlignment.MiddleCenter;
            unfriendButton.BackColor = this.BackColor;
            unfriendButton.ForeColor = Color.FromArgb(234, 234, 234);
            unfriendButton.Font = CustomFonts.Smallest;
            unfriendButton.Height = blockFriendButton.Height;
            unfriendButton.MouseEnter += (sm, me) => { unfriendButton.BackColor = Color.FromArgb(160, 0, 0); };
            unfriendButton.MouseLeave += (sm, me) => { unfriendButton.BackColor = this.BackColor; };
            this.Controls.Add(unfriendButton);
        }

        private void UnfriendifyFriend()
        {
            BackgroundWorker buttonLoader = new BackgroundWorker();
            buttonLoader.DoWork += (s, e) =>
                {
                    bool unfriended = true;// UserRepository.Instance.Unfriend(user);
                    Universal.ParentForm.Invoke(new Action(() => 
                    {
                        if (unfriended)
                        {
                            unfriendButton.Text = "Unfriended";
                            unfriendButton.Width = unfriendButton.PreferredWidth;
                            unfriendButton.Enabled = false;

                            this.ReviceUserInfoPanel(null);
                            this.ResetLocations();
                        }
                    }));
                };
            buttonLoader.RunWorkerAsync();
        }

        private void ReviceUserInfoPanel(Time addedOn)
        {
            //if (addedOn == null)
            //{
            //    try
            //    {
            //        genderLabel.Dispose();
            //        birthdateLabel.Dispose();
            //        friendSinceLabel.Dispose();
            //    }
            //    catch { }
            //    genderLabel = null;
            //    birthdateLabel = null;
            //    friendSinceLabel = null;
            //    return;
            //}
            //if (user.GenderIndex != null)
            //{
            //    genderLabel = new Label();
            //    genderLabel.Text = (new string[] { "Male", "Female", "Other" })[(int)user.GenderIndex];
            //    genderLabel.Font = CustomFonts.Smaller;
            //    genderLabel.Size = genderLabel.PreferredSize;
            //    genderLabel.ForeColor = Color.FromArgb(234, 234, 234);
            //    this.Controls.Add(genderLabel);
            //}

            //if (user.Birthdate != null)
            //{
            //    birthdateLabel = new Label();
            //    birthdateLabel.Text = user.Birthdate.LongDate;
            //    birthdateLabel.Font = CustomFonts.Smaller;
            //    birthdateLabel.Size = birthdateLabel.PreferredSize;
            //    birthdateLabel.ForeColor = Color.FromArgb(234, 234, 234);
            //    this.Controls.Add(birthdateLabel);
            //}

            //friendSinceLabel = new Label();
            //friendSinceLabel.Text = "Friend since " + addedOn.LongDate;
            //friendSinceLabel.Font = CustomFonts.Smallest;
            //friendSinceLabel.Size = friendSinceLabel.PreferredSize;
            //friendSinceLabel.ForeColor = Color.FromArgb(234, 234, 234);
            //this.Controls.Add(friendSinceLabel);
        }

        private void CheckifyFriend()
        {
            //BackgroundWorker buttonLoader = new BackgroundWorker();
            //buttonLoader.DoWork += (s, e) =>
            //{
            //    Time addedFriendOn = UserRepository.Instance.FriendAddedTime(user);
            //    Universal.ParentForm.Invoke(new Action(() =>
            //    {
            //        unfriendButton.BackColor = this.BackColor;
            //        if (addedFriendOn != null)
            //        {
            //            unfriendButton.Text = "Unfriend";
            //            unfriendButton.BackColor = this.BackColor;
            //            unfriendButton.Enabled = true;
            //        }
            //        else
            //        {
            //            unfriendButton.Text = "Not friend";
            //            unfriendButton.BackColor = Color.FromArgb(190,190,190);
            //            unfriendButton.Enabled = false;
            //        }
            //        unfriendButton.Width = unfriendButton.PreferredWidth;
            //        ReviceUserInfoPanel(addedFriendOn);
            //        this.ResetLocations();
            //    }));
            //};
            //buttonLoader.RunWorkerAsync();
        }

        private void ResetLocations()
        {
            //profileIconLabel, nameLabel, usernameLabel, genderLabel, birthdateLabel, friendSinceLabel, blockFriendButton, unfriendButton;
            int availableTop = 0;
            profileIconLabel.Location = new Point(5, availableTop + 5);
            availableTop = profileIconLabel.Bottom;
            nameLabel.Location = new Point(profileIconLabel.Left, availableTop + 5);
            availableTop = nameLabel.Bottom;
            usernameLabel.Location = new Point(5, availableTop + 5);
            availableTop = usernameLabel.Bottom;

            blockFriendButton.Location = new Point(this.Width - blockFriendButton.Width - 2, profileIconLabel.Top);
            unfriendButton.Width = unfriendButton.PreferredWidth;
            unfriendButton.Location = new Point(this.blockFriendButton.Left - unfriendButton.Width - 2, profileIconLabel.Top);

            if(genderLabel != null)
            {
                genderLabel.Location = new Point(5, availableTop + 5);
                availableTop = genderLabel.Bottom;
            }

            if (birthdateLabel != null)
            {
                birthdateLabel.Location = new Point(5, availableTop + 5);
                availableTop = birthdateLabel.Bottom;
            }

            if (friendSinceLabel != null)
            {
                friendSinceLabel.Location = new Point(5, availableTop + 5);
                availableTop = friendSinceLabel.Bottom;
            }

            this.Height = this.PreferredSize.Height + 5;
        }
    }
}

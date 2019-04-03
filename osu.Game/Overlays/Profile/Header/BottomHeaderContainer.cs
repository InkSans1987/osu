﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Users;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Overlays.Profile.Header
{
    public class BottomHeaderContainer : CompositeDrawable
    {
        private LinkFlowContainer bottomTopLinkContainer;
        private LinkFlowContainer bottomLinkContainer;
        private Color4 linkBlue, communityUserGrayGreenLighter;

        public readonly Bindable<User> User = new Bindable<User>();

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AutoSizeAxes = Axes.Y;
            User.ValueChanged += e => updateDisplay(e.NewValue);

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colours.CommunityUserGrayGreenDarker,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Padding = new MarginPadding { Horizontal = UserProfileOverlay.CONTENT_X_MARGIN, Vertical = 10 },
                    Spacing = new Vector2(0, 10),
                    Children = new Drawable[]
                    {
                        bottomTopLinkContainer = new LinkFlowContainer(text => text.TextSize = 12)
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        },
                        bottomLinkContainer = new LinkFlowContainer(text => text.TextSize = 12)
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        }
                    }
                }
            };

            linkBlue = colours.BlueLight;
            communityUserGrayGreenLighter = colours.CommunityUserGrayGreenLighter;
        }

        private void updateDisplay(User user)
        {
            void bold(SpriteText t) => t.Font = @"Exo2.0-Bold";
            void addSpacer(OsuTextFlowContainer textFlow) => textFlow.AddArbitraryDrawable(new Container { Width = 15 });

            bottomTopLinkContainer.Clear();
            bottomLinkContainer.Clear();

            if (user == null) return;

            if (user.JoinDate.ToUniversalTime().Year < 2008)
            {
                bottomTopLinkContainer.AddText("Here since the beginning");
            }
            else
            {
                bottomTopLinkContainer.AddText("Joined ");
                bottomTopLinkContainer.AddText(new DrawableDate(user.JoinDate), bold);
            }

            addSpacer(bottomTopLinkContainer);

            if (user.PlayStyles?.Length > 0)
            {
                bottomTopLinkContainer.AddText("Plays with ");
                bottomTopLinkContainer.AddText(string.Join(", ", user.PlayStyles.Select(style => style.GetDescription())), bold);

                addSpacer(bottomTopLinkContainer);
            }

            if (user.LastVisit.HasValue)
            {
                bottomTopLinkContainer.AddText("Last seen ");
                bottomTopLinkContainer.AddText(new DrawableDate(user.LastVisit.Value), bold);

                addSpacer(bottomTopLinkContainer);
            }

            bottomTopLinkContainer.AddText("Contributed ");
            bottomTopLinkContainer.AddLink($@"{user.PostCount:#,##0} forum posts", $"https://osu.ppy.sh/users/{user.Id}/posts", creationParameters: bold);

            void tryAddInfo(IconUsage icon, string content, string link = null)
            {
                if (string.IsNullOrEmpty(content)) return;

                bottomLinkContainer.AddIcon(icon, text =>
                {
                    text.TextSize = 10;
                    text.Colour = communityUserGrayGreenLighter;
                });
                if (link != null)
                {
                    bottomLinkContainer.AddLink(" " + content, link, creationParameters: text =>
                    {
                        bold(text);
                        text.Colour = linkBlue;
                    });
                }
                else
                    bottomLinkContainer.AddText(" " + content, bold);

                addSpacer(bottomLinkContainer);
            }

            string websiteWithoutProtcol = user.Website;
            if (!string.IsNullOrEmpty(websiteWithoutProtcol))
            {
                int protocolIndex = websiteWithoutProtcol.IndexOf("//", StringComparison.Ordinal);
                if (protocolIndex >= 0)
                    websiteWithoutProtcol = websiteWithoutProtcol.Substring(protocolIndex + 2);
            }

            tryAddInfo(FontAwesome.Solid.MapMarker, user.Location);
            tryAddInfo(OsuIcon.Heart, user.Interests);
            tryAddInfo(FontAwesome.Solid.Suitcase, user.Occupation);
            bottomLinkContainer.NewLine();
            if (!string.IsNullOrEmpty(user.Twitter))
                tryAddInfo(FontAwesome.Brands.Twitter, "@" + user.Twitter, $@"https://twitter.com/{user.Twitter}");
            tryAddInfo(FontAwesome.Brands.Discord, user.Discord);
            tryAddInfo(FontAwesome.Brands.Skype, user.Skype, @"skype:" + user.Skype + @"?chat");
            tryAddInfo(FontAwesome.Brands.Lastfm, user.Lastfm, $@"https://last.fm/users/{user.Lastfm}");
            tryAddInfo(FontAwesome.Solid.Link, websiteWithoutProtcol, user.Website);
        }
    }
}

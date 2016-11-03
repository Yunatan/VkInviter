using System;
using System.Collections.Generic;
using NDesk.Options;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkInviter
{
    internal class Program
    {
        private static VkApi vkApi;
        private static string groupIdToInviteFrom;
        private static long groupIdToInveiteTo;
        private static int membersOffset;

        private static void Main(string[] args)
        {
            SetupApiWrapper(args);

            var groupMembersList = GetMembersToInvite();

            Console.WriteLine("Total people to try to invite: " + groupMembersList.Count);

            SendInvites(groupMembersList);

            Console.WriteLine("Inviting process finished.");
        }

        private static void SetupApiWrapper(string[] args)
        {
            var vkParams = ParseParams(args);
            vkApi.RequestsPerSecond = 3;
            vkApi.Authorize(vkParams);
        }

        private static ApiAuthParams ParseParams(string[] args)
        {
            var vkParams = new ApiAuthParams {Settings = Settings.Groups};
            var options = new OptionSet
            {
                {"id=|vkAppId=", "vkApi Application ID", x => vkParams.ApplicationId = ulong.Parse(x)},
                {"l=|vkLogin=", "vkApi User Login", x => vkParams.Login = x},
                {"p=|vkPassword=", "vkApi User Password", x => vkParams.Password = x},
                {"to=|inviteTo=", "Group ID To Invite To", x => groupIdToInveiteTo = long.Parse(x)},
                {"from=|inviteFrom=", "Group ID To Invite From", x => groupIdToInviteFrom = x},
                {"ag=|antiGateId=", "AntiGate Id", x => vkApi = new VkApi(new AntiGateCaptchaResolver(x))},
                {"of:|offset:", "Ofsset to start iteration of group members", x => membersOffset = int.Parse(x)}
            };
            options.Parse(args);
            return vkParams;
        }

        private static List<User> GetMembersToInvite()
        {
            var membersCount = GetGroupMembersCount();

            var iterations = membersCount / 1000 + (membersCount % 1000 == 0 ? 0 : 1);

            var members = new List<User>();
            for (var i = 0; i < iterations; i++)
            {
                members.AddRange(vkApi.Groups.GetMembers(new GroupsGetMembersParams { GroupId = groupIdToInviteFrom, Offset = i * 1000 }));
            }

            return members;
        }

        private static int GetGroupMembersCount()
        {
            int membersCount;
            vkApi.Groups.GetMembers(out membersCount, new GroupsGetMembersParams { Count = 0, GroupId = groupIdToInviteFrom }); 
            return membersCount;
        }

        private static void SendInvites(IList<User> groupMembersList)
        {
            for (var i = membersOffset; i < groupMembersList.Count; i++)
            {
                var response = TryToInvite(groupMembersList[i]);
                Console.WriteLine("{0}) id{1} – {2}", i + 1, groupMembersList[i].Id, response);
            }
        }

        private static string TryToInvite(User groupMember)
        {
            try
            {
                vkApi.Groups.Invite(groupIdToInveiteTo, groupMember.Id);
            }
            catch (VkNet.Exception.AccessDeniedException e)
            {
                return e.Message.Replace("Access denied: ", string.Empty);
            }
            return "successfully invited!";
        }
    }
}

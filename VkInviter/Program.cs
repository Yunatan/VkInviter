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

        private static void Main(string[] args)
        {
            var vkParams = new ApiAuthParams {Settings = Settings.Groups};

            vkParams = ParseParams(args, vkParams);

            vkApi.Authorize(vkParams);

            var groupMembersList = GetMembersToInvite();

            Console.WriteLine("Total people to try to invite: " + groupMembersList.Count);

            for (var i = 0; i < groupMembersList.Count; i++)
            {
                var response = TryToInvite(groupMembersList[i]);
                Console.WriteLine("{0}) id{1} – {2}", i + 1, groupMembersList[i].Id, response);
            }

            Console.WriteLine("Inviting process finished.");
            Console.ReadKey();
        }

        private static ApiAuthParams ParseParams(string[] args, ApiAuthParams vkParams)
        {
            var options = new OptionSet
            {
                {"id=|vkAppId=", "vkApi Application ID", x => vkParams.ApplicationId = ulong.Parse(x)},
                {"l=|vkLogin=", "vkApi User Login", x => vkParams.Login = x},
                {"p=|vkPassword=", "vkApi User Password", x => vkParams.Password = x},
                {"to=|inviteTo=", "Group ID To Invite To", x => groupIdToInveiteTo = long.Parse(x)},
                {"from=|inviteFrom=", "Group ID To Invite From", x => groupIdToInviteFrom = x},
                {"ag=|antiGateId=", "AntiGate Id", x => vkApi = new VkApi(new AntiGateCaptchaResolver(x))}
            };
            options.Parse(args);
            return vkParams;
        }

        private static string TryToInvite(User groupMember)
        {
            bool success;
            try
            {
                success = vkApi.Groups.Invite(groupIdToInveiteTo, groupMember.Id);
            }
            catch (VkNet.Exception.AccessDeniedException e)
            {
                return e.Message.Replace("Access denied: ", string.Empty);
            }
            return success ? "successfully invited!" : "unknown error :(";
        }

        private static List<User> GetMembersToInvite()
        {
            var members = new List<User>();
            int membersCount;
            vkApi.Groups.GetMembers(out membersCount, new GroupsGetMembersParams { Count = 0, GroupId = groupIdToInviteFrom }); // get to know number of members

            var iterations = membersCount/1000 + (membersCount%1000 == 0 ? 0 : 1);

            for (var i = 0; i < iterations; i++)
            {
                members.AddRange(vkApi.Groups.GetMembers(new GroupsGetMembersParams { GroupId = groupIdToInviteFrom, Offset = i * 1000 }));
            }

            return members;
        }
    }
}

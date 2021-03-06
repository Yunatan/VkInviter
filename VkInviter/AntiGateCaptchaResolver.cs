﻿using System;
using System.Net;
using Akumu.Antigate;
using VkNet.Utils.AntiCaptcha;

namespace VkInviter
{
    internal class AntiGateCaptchaResolver : ICaptchaSolver
    {
        private readonly AntiCaptcha anticapApi;

        public AntiGateCaptchaResolver(string anticapKey)
        {
            anticapApi = new AntiCaptcha(anticapKey);
        }

        public string Solve(string url)
        {
            Console.WriteLine("Wait a second, resolving captcha...");
            string answer = null;

            try
            {
                answer = anticapApi.GetAnswer(GetImageByUrl(url));
            }
            catch (AntigateErrorException aee)
            {
                Console.WriteLine("Antigate error: {0}", aee.Message);
            }

            Console.WriteLine("Captcha resolved.");
            return answer;
        }

        private byte[] GetImageByUrl(string url)
        {
            byte[] imageAsByteArray;
            using (var webClient = new WebClient())
            {
                imageAsByteArray = webClient.DownloadData(url);
            }
            return imageAsByteArray;
        }

        public void CaptchaIsFalse()
        {
            anticapApi.FalseCaptcha();
        }
    }
}
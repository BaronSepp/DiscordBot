﻿using Bot.Handlers;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Modules
{
    [Name("Image")]
    public class ImageModule : ModuleBase<SocketCommandContext>
    {
        private readonly PictureHandler _pictureService;
        private readonly ILogger<ImageModule> _logger;

        public ImageModule(PictureHandler pictureService, ILogger<ImageModule> logger)
        {
            _pictureService = pictureService;
            _logger = logger;
        }

        [Command("cat")]
        public async Task CatAsync()
        {
            var stream = await _pictureService.GetCatPictureAsync();
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("r34")]
        [Alias("rule34", "34")]
        [RequireNsfw]
        public async Task Rule34([Remainder] string text)
        {
            try
            {
                var textArray = text.Split(' ');
                var stream = await _pictureService.GetRule34(textArray);
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "rule34.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                await Context.Channel.SendMessageAsync("Nothing found.");
            }

        }

        [Command("anime")]
        [Alias("safe")]
        public async Task Anime([Remainder] string text)
        {
            try
            {
                var textArray = text.Split(' ');
                var stream = await _pictureService.GetAnime(textArray);
                stream.Seek(0, SeekOrigin.Begin);
                await Context.Channel.SendFileAsync(stream, "anime.png");
            }
            catch (Exception ex)
            {
                await Context.Channel.SendMessageAsync("Nothing found.");
                _logger.LogError(ex.Message, ex);
            }
        }
    }

}

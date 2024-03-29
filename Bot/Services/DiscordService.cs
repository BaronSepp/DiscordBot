using Bot.Handlers;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Bot.Services;

public sealed class DiscordService : IHostedService
{
	private readonly DiscordSocketClient _discordSocketClient;
	private readonly IAudioService _audioService;
	private readonly IConfiguration _configuration;
	private readonly InteractionService _interactionService;
	private readonly InteractionHandler _interactionHandler;
	private readonly LoggingHandler _loggingHandler;
	private readonly InactivityTrackingService _inactivityTrackingService;

	public DiscordService(
		DiscordSocketClient discordSocketClient,
		IAudioService audioService,
		IConfiguration configuration,
		InteractionService interactionService,
		InteractionHandler interactionHandler,
		LoggingHandler loggingHandler,
		InactivityTrackingService inactivityTrackingService)
	{
		_discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
		_audioService = audioService ?? throw new ArgumentNullException(nameof(audioService));
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
		_interactionHandler = interactionHandler ?? throw new ArgumentNullException(nameof(interactionHandler));
		_loggingHandler = loggingHandler ?? throw new ArgumentNullException(nameof(loggingHandler));
		_inactivityTrackingService = inactivityTrackingService ?? throw new ArgumentNullException(nameof(inactivityTrackingService));
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_discordSocketClient.Log += _loggingHandler.Log;
		_interactionService.Log += _loggingHandler.Log;

		// Load token from environment
		await _discordSocketClient.LoginAsync(TokenType.Bot, _configuration["DiscordToken"]);
		await _discordSocketClient.SetGameAsync("/help");

		// Start Clients
		await _discordSocketClient.StartAsync();

		// Discord Audio
		await _audioService.InitializeAsync();
		_inactivityTrackingService.BeginTracking();

		// Register commands
		await _interactionHandler.InitializeAsync();
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await _discordSocketClient.LogoutAsync();
		_inactivityTrackingService.StopTracking();

		_inactivityTrackingService.Dispose();
		_audioService.Dispose();
		_discordSocketClient.Dispose();
	}

}
# nullbot-net

Commands are prefixed as either `n!` or `nt!` depending on whether the bot is running as Release or Development.

## Discord.Net vs DSharpPlus

This repo contains two implementations, one with Discord.Net and the other with DSharpPlus.

### Discord.Net

Website: https://discordnet.dev/
GitHub: https://github.com/discord-net/Discord.Net

Supports:
- Commands
- Slash Commands via InteractionService
- Context Commands via InteractionService

### DSharpPlus

Website: https://dsharpplus.github.io/
GitHub: https://github.com/DSharpPlus/DSharpPlus

Supports:
- Commands via `DSharpPlus.CommandsNext`
- Slash Commands via `DSharpPlus.SlashCommands`

Features:
- Creates a default `help` command

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace LovecsVfxLib.LovecsVfxLibCode.Vfx;

public partial class NLovecSimpleVfx : Node2D
{
	[Export] public int LifetimeMs = 1000;

	[Export] public bool RestartGpuParticles = true;
	[Export] public bool RestartCpuParticles = true;
	[Export] public bool PlayAnimatedSprites = true;
	[Export] public bool PlayAnimationPlayer = true;

	[Export] public string AnimationPlayerName = "AnimationPlayer";
	[Export] public string AnimationName = "animation";

	private CancellationTokenSource? _cts;

	public override void _ExitTree()
	{
		_cts?.Cancel();
		_cts?.Dispose();
		_cts = null;
	}

	public override void _Ready()
	{
		if (RestartGpuParticles)
			RestartAllGpuParticles();

		if (RestartCpuParticles)
			RestartAllCpuParticles();

		if (PlayAnimatedSprites)
			PlayAllAnimatedSprites();

		if (PlayAnimationPlayer)
			PlayRootAnimationPlayer();

		if (LifetimeMs > 0)
			TaskHelper.RunSafely(DeleteAfterComplete());
	}

	private void RestartAllGpuParticles()
	{
		foreach (var particles in FindNodes<GpuParticles2D>(this))
		{
			particles.Restart();
			particles.Emitting = true;
		}
	}

	private void RestartAllCpuParticles()
	{
		foreach (var particles in FindNodes<CpuParticles2D>(this))
		{
			particles.Restart();
			particles.Emitting = true;
		}
	}

	private void PlayAllAnimatedSprites()
	{
		foreach (var sprite in FindNodes<AnimatedSprite2D>(this))
		{
			if (sprite.SpriteFrames != null)
				sprite.Play();
		}
	}

	private void PlayRootAnimationPlayer()
	{
		var animationPlayer = GetNodeOrNull<AnimationPlayer>(AnimationPlayerName);

		if (animationPlayer == null)
			return;

		if (animationPlayer.HasAnimation(AnimationName))
			animationPlayer.Play(AnimationName);
	}

	private static IEnumerable<T> FindNodes<T>(Node node) where T : Node
	{
		foreach (var child in node.GetChildren())
		{
			if (child is T typedChild)
				yield return typedChild;

			foreach (var nestedChild in FindNodes<T>(child))
				yield return nestedChild;
		}
	}

	private async Task DeleteAfterComplete()
	{
		_cts = new CancellationTokenSource();

		try
		{
			await Task.Delay(LifetimeMs, _cts.Token);
			this.QueueFreeSafely();
		}
		catch (TaskCanceledException)
		{
			// VFX was removed before its lifetime ended.
		}
	}
}

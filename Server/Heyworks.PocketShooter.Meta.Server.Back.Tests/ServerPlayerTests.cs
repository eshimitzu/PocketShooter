using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using NUnit.Framework;

namespace Heyworks.PocketShooter
{
    public class ServerPlayerTests
    {
        [Test]
        public void ApplyReward_ExperienceInLevelChanged_LevelNotChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var player = new ServerPlayer(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default", timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 10);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore + reward.Experience, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelNotZero_ExperienceInLevelChanged_LevelNotChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var playerState = new ServerPlayerState(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default")
            {
                ExperienceInLevel = 190,
            };

            var player = new ServerPlayer(playerState, timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 9);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore + reward.Experience, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelNotChanged_LevelChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var player = new ServerPlayer(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default", timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 200);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore + 1, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelNotZero_ExperienceInLevelNotChanged_LevelChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var playerState = new ServerPlayerState(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default")
            {
                ExperienceInLevel = 199,
            };

            var player = new ServerPlayer(playerState, timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 200 + 400 + 600);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore + 3, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelChanged_LevelChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var player = new ServerPlayer(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default", timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 200 + 400 + 600 + 20);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore + 20, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore + 3, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelNotZero_ExperienceInLevelChanged_LevelChanged()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var playerState = new ServerPlayerState(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default")
            {
                ExperienceInLevel = 199,
            };

            var player = new ServerPlayer(playerState, timeProvider, playerConfiguration);

            var levelBefore = player.Level;
            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, 200 + 400 + 600 + 1);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore + 1, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(levelBefore + 3, Is.EqualTo(player.Level));
        }

        [Test]
        public void ApplyReward_ExperienceInLevelNotChanged_MaxLevelGained()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var player = new ServerPlayer(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default", timeProvider, playerConfiguration);

            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, int.MaxValue);

            player.ApplyReward(reward);

            Assert.That(experienceInLevelBefore, Is.EqualTo(player.ExperienceInLevel));
            Assert.That(player.HasMaxLevel, Is.True);
        }

        [Test]
        public void ApplyReward_ExperienceInLevelChanged_MaxLevelGained()
        {
            var timeProvider = new SystemTimeProvider();
            var config = new ServerGameConfig();
            var playerConfiguration = new PlayerConfiguration(config);

            var playerState = new ServerPlayerState(Guid.NewGuid(), "player", Guid.NewGuid().ToString(), "default")
            {
                ExperienceInLevel = 199,
            };

            var player = new ServerPlayer(playerState, timeProvider, playerConfiguration);

            var experienceInLevelBefore = player.ExperienceInLevel;
            var reward = new PlayerReward(0, 0, int.MaxValue);

            player.ApplyReward(reward);

            Assert.That(player.ExperienceInLevel, Is.Zero);
            Assert.That(player.HasMaxLevel, Is.True);
        }
    }
}

# Hungry Snake (WIP)

## Summary

Hungry Snake (working title) is a game inspired by the classic *Snake*. The player controls a snake that is always moving. The player will be tasked with changing the direction of the snake into food and away from enemies or obstacles. The loop is that you will grow larger, making it harder to protect your tail from danger, including your own hungry mouth. If you run into your tail, you consume the tail segment and those after it. This also starts a 'bleed out' timer that will slowly destroy tail segments, and you if there are none left. The goal is to survive until time runs out or you meet the conditions for an alternative win. The game is very much a work-in-progress. It's still early in development and I work on it everyday.

## Why

Hungry Snake (working title) started as a snake clone that I built to gain experience making games in Unity. It was my third clone made in Unity after Flappy Bird and Breakout. Snake was the first one that gave me ideas. Instead of the player losing when they collided with their tail, I wanted to build a system that allowed the player to eat it, then regrow it. This led to more ideas and now I have a vision for my first final, complete, game.

## Mechanics

### Movement

The snake is always moving but the player controls the direction using WASD. Eventually, I will make keys rebindable and controller support.

### Stats

The player will have stats that affect gameplay.

#### Movement Speed

Movement speed controls how often the snake will move by adjusting the timer. This will be considered a nerf to the player's ability more often than a buff. While you may be able to reach food faster, you'll also have less time to react. You might hit your tail or run into a fire that burns you ticks your tail down faster.

#### Strength

Strength will be the stat that affects how the player will interact with enemies and obstacles. Without enough strength, the player will bounce off of a stone in a random direction. With enough, they'll be able to move or even break the stone. The only traditional 'enemy' in the game with be the hunter. Strength will make it easier to take them out.

#### Resistance

Resistance will govern how much damage you take. This includes everything from hunter attacks to the bleed out mechanic that shrinks your tail.

#### Mutation

I'm still thinking about mutation. I want the effect to be dynamic but I'm still thinking of effects besides random stat boosts.

### Tail Growth

The player can grow their tail by eating food. Right now, food just grows one tail segment but I will add different types of food in the future. Some will be the same item, just stronger. Some might run from you (mice). Some will move together in groups (ants). All of them will have an impact on your stats.

To be continued...

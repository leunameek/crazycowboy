# Death System Setup Guide

## Overview
This guide explains how to implement the death system for your cowboy character, including death animations, hazard detection, and respawning.

## Components Created

### Hazard.cs
**Location**: `Assets/Scripts/Hazard.cs`

Simple component that instantly kills the player on contact. Features:
- Instant death on collision
- Automatic respawn after delay
- Optional visual flashing effect
- Works with trigger colliders (separate from tilemap)
- Debug visualization in Scene view

## Step-by-Step Setup

### Part 1: Player Setup

#### 1. Add Required Components to Player

Select your player GameObject and add:

1. **Collider2D** (if not already present):
   - Add Component → Physics 2D → Box Collider 2D (or Circle Collider 2D)
   - **Important**: Uncheck "Is Trigger" (player needs solid collision)
   - Adjust size to match your character sprite

2. **Rigidbody2D** (if not already present):
   - Add Component → Physics 2D → Rigidbody 2D
   - Set **Body Type** to `Kinematic` (we handle movement manually)
   - Set **Collision Detection** to `Continuous`
   - Check **Freeze Rotation Z** (prevent spinning)

#### 2. Set Player Tag

**Important**: Your player GameObject must have the "Player" tag:
1. Select player GameObject
2. In Inspector, click **Tag** dropdown (top)
3. Select **Player** (or create it if it doesn't exist)

### Part 2: Create Death Animation

#### 1. Prepare Death Sprite Sheet

You already have `Sprites/Death_vaquero.png`. Follow these steps:

1. Select the sprite in Project window
2. Set **Sprite Mode** to `Multiple`
3. Click **Sprite Editor**
4. Slice the sprite sheet into individual frames
5. Click **Apply**

#### 2. Create Death Animation Clip

1. Right-click in `Assets/Sprites` folder
2. Create → Animation
3. Name it `Death`
4. Open Animation window (Window → Animation → Animation)
5. Select your player GameObject
6. Select the `Death` animation clip
7. Drag all death sprite frames into the timeline
8. Set appropriate sample rate (e.g., 12 fps)
9. **Important**: Uncheck **Loop** (death animation should play once)

#### 3. Update Animator Controller

Open your Animator Controller (`Idle_vaquero_0.controller`):

**Add Parameter**:
- Name: `IsDead`
- Type: `Bool`

**Create Death State**:
1. Drag the `Death` animation into the Animator window
2. This creates a new state

**Create Transitions**:

**Any State → Death**:
1. Right-click **Any State** → Make Transition → Click Death
2. In Inspector:
   - Uncheck **Has Exit Time**
   - Add condition: `IsDead` **true**
   - Set **Transition Duration** to `0` (instant transition)

**Death → Idle** (for respawn):
1. Right-click Death → Make Transition → Click Idle
2. In Inspector:
   - Check **Has Exit Time** ✓
   - Set **Exit Time** to `1.0` (end of animation)
   - Add condition: `IsDead` **false**

### Part 3: Create Hazards

#### Method 1: Simple Hazard Object

1. **Create GameObject**:
   - Right-click in Hierarchy → Create Empty
   - Name it "Hazard_Spikes" (or whatever fits your hazard)

2. **Add Visual** (optional):
   - Add Component → Sprite Renderer
   - Assign a sprite for your hazard

3. **Add Collider**:
   - Add Component → Box Collider 2D (or Circle Collider 2D)
   - **Important**: Check **Is Trigger** ✓
   - Adjust size to cover the dangerous area

4. **Add Hazard Component**:
   - Add Component → Hazard
   - Configure settings (see below)

5. **Set Tag**:
   - Set GameObject tag to **Hazard** or **DeathZone**

#### Method 2: Death Zone (Instant Kill)

For pits or areas that should instantly kill:

1. Create GameObject named "DeathZone"
2. Add Box Collider 2D
3. Check **Is Trigger** ✓
4. Stretch it across the bottom of your level
5. Add Hazard component
6. Check **Instant Kill** ✓
7. Set tag to **DeathZone**

### Part 4: Configure Hazard Component

**Visual Feedback** (optional):
- **Hazard Color**: Red (color to flash)
- **Enable Flashing**: Check to make hazard flash
- **Flash Speed**: `2` (speed of flashing effect)

**Note**: All hazards now cause instant death - no additional configuration needed!

### Part 5: Unity Tags Setup

Ensure these tags exist in your project:

1. Go to **Edit → Project Settings → Tags and Layers**
2. Add these tags if they don't exist:
   - `Player`
   - `Hazard`
   - `DeathZone`
   - `Enemy` (optional, for future use)

### Part 6: Layer Setup (Optional but Recommended)

To prevent hazards from interfering with tilemap collision:

1. **Create Layers**:
   - Edit → Project Settings → Tags and Layers
   - Add layers: `Player`, `Hazard`, `Ground`

2. **Assign Layers**:
   - Player GameObject → Layer: `Player`
   - Hazard GameObjects → Layer: `Hazard`
   - Tilemap GameObject → Layer: `Ground`

3. **Configure Collision Matrix**:
   - Edit → Project Settings → Physics 2D
   - Scroll to **Layer Collision Matrix**
   - Uncheck collision between `Hazard` and `Ground`
   - Keep collision between `Player` and `Hazard`

## Testing Your Death System

### Test Checklist

1. **Enter Play Mode**
2. **Test Death**:
   - Move player into hazard
   - Death animation should play
   - Player should respawn after delay
3. **Test Invincibility**:
   - After respawn, player should be invincible briefly
   - Try touching hazard immediately after respawn
4. **Test Multiple Deaths**:
   - Die multiple times to ensure respawning works consistently

## Advanced Features

### Custom Respawn Points

Create a simple checkpoint system by modifying the Hazard.cs respawn position:

```csharp
public class Checkpoint : MonoBehaviour
{
    public static Vector3 respawnPoint = Vector3.zero;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            respawnPoint = transform.position;
        }
    }
}
```

Then modify the Hazard.cs RespawnPlayer method to use this checkpoint.

### Death Sound Effects

Add AudioSource component to your player and create a death sound script:

```csharp
public class PlayerSounds : MonoBehaviour
{
    public AudioClip deathSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Call this from Hazard.cs when player dies
    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound);
    }
}
```

### Different Hazard Types

All hazards now cause instant death, but you can create different visual types:

**Spike Trap**:
- Add spike sprite
- Enable flashing for warning effect

**Lava Pool**:
- Add lava sprite
- Use orange/red flashing color

**Death Pit** (invisible):
- No sprite renderer
- Just the collider stretched across the pit

## Troubleshooting

### Player Not Dying

**Check**:
- Player has Collider2D (not trigger)
- Player has Rigidbody2D (Kinematic)
- Player tag is set to "Player"
- Hazard has Collider2D with "Is Trigger" checked
- Hazard tag is "Hazard" or "DeathZone"

### Death Animation Not Playing

**Check**:
- Death animation clip exists
- Animator Controller has "IsDead" parameter (Bool)
- Transition from Any State to Death exists
- Transition condition is `IsDead == true`
- PlayerAnimationController component is on player

### Player Not Respawning

**Check**:
- Respawn Delay is set (not 0)
- Check Console for errors
- Ensure MainController and PlayerMovement are being re-enabled

### Hazard Affecting Tilemap

**Solution**:
- Set up layers as described in Part 6
- Configure Layer Collision Matrix
- Ensure hazard collider is on separate layer from ground

## Component Integration

The death system integrates with your refactored components:

```
Player GameObject
├── MainController (disabled on death, re-enabled on respawn)
├── PlayerMovement (disabled on death, re-enabled on respawn)
├── PlayerAnimationController (triggers death animation)
├── CollisionResolver
├── TilemapCollisionDetector
├── Collider2D (for tilemap collision)
├── Rigidbody2D (Kinematic)
└── Hazard Objects (separate GameObjects with trigger colliders)
```

## Example Hazard Setups

### Spike Trap
```
GameObject: Spike_Trap
├── Sprite Renderer (spike sprite)
├── Box Collider 2D (Is Trigger: ✓)
├── Hazard Component
│   ├── Enable Flashing: ✓
│   ├── Hazard Color: Red
│   └── Flash Speed: 2
└── Tag: Hazard
```

### Lava Pool
```
GameObject: Lava_Pool
├── Sprite Renderer (lava sprite)
├── Box Collider 2D (Is Trigger: ✓)
├── Hazard Component
│   ├── Enable Flashing: ✓
│   ├── Hazard Color: Orange
│   └── Flash Speed: 1.5
└── Tag: Hazard
```

### Death Pit
```
GameObject: Death_Pit
├── Box Collider 2D (Is Trigger: ✓, stretched across pit)
├── Hazard Component
│   └── Enable Flashing: ✗ (invisible hazard)
└── Tag: DeathZone
```

## Summary

You now have a complete death system that:
- ✓ Detects hazard collisions
- ✓ Plays death animation
- ✓ Respawns player automatically
- ✓ Provides invincibility frames
- ✓ Supports multiple hazard types
- ✓ Integrates with your refactored component system
- ✓ Uses Unity Events for extensibility

The system is modular and easy to extend with new features like checkpoints, health pickups, or different enemy types!
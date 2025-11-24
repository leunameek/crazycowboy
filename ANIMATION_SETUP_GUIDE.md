# Player Animation Setup Guide

## Overview
This guide will help you set up animations for your cowboy character using the sprite sheets you have:
- `Idle_vaquero.png` - Idle animation
- `Caminata_vaquero.png` - Walking animation
- `Jump_vaquero.png` - Jumping animation
- `Death_vaquero.png` - Death animation (optional)

## Component Created

### PlayerAnimationController.cs
**Location**: `Assets/Scripts/PlayerAnimationController.cs`

This component automatically updates animator parameters based on the player's movement state. It integrates seamlessly with your refactored `PlayerMovement` component.

## Step-by-Step Setup

### Step 1: Prepare Sprite Sheets

1. **Select each sprite sheet** in the Project window:
   - `Sprites/Idle_vaquero.png`
   - `Sprites/Caminata_vaquero.png`
   - `Sprites/Jump_vaquero.png`

2. **Configure sprite settings** (for each sprite sheet):
   - In the Inspector, set **Sprite Mode** to `Multiple`
   - Set **Pixels Per Unit** to match your game (typically 16, 32, or 100)
   - Click **Apply**

3. **Slice the sprite sheets**:
   - Click **Sprite Editor** button
   - Click **Slice** dropdown → **Automatic** or **Grid By Cell Count**
   - Adjust the grid to match your sprite frames
   - Click **Apply**

### Step 2: Create Animation Clips

#### Create Idle Animation
1. Right-click in `Assets/Sprites` folder
2. Select **Create → Animation**
3. Name it `Idle`
4. Open the Animation window (Window → Animation → Animation)
5. Select your player GameObject
6. In Animation window, select the `Idle` clip
7. Drag all idle sprite frames into the timeline
8. Set sample rate (e.g., 12 for 12 fps)

#### Create Walk Animation
1. Create new animation: `Walk`
2. Drag all walking sprite frames into the timeline
3. Adjust timing as needed

#### Create Jump Animation
1. Create new animation: `Jump`
2. Drag jump sprite frames into the timeline
3. This can be a short animation or even a single frame

#### Optional: Create Fall Animation
If your jump sprite sheet has separate fall frames:
1. Create new animation: `Fall`
2. Use the falling frames

### Step 3: Set Up Animator Controller

#### Open Animator Controller
1. Double-click `Sprites/Idle_vaquero_0.controller` (or create a new one)
2. This opens the Animator window

#### Create Animation States
1. **Drag animation clips** into the Animator window:
   - Idle
   - Walk
   - Jump
   - Fall (optional)

2. **Set Idle as default state**:
   - Right-click Idle state → **Set as Layer Default State**
   - It should turn orange

#### Add Parameters
In the Animator window, click the **Parameters** tab and add:

| Parameter Name | Type  | Description |
|---------------|-------|-------------|
| Speed         | Float | Horizontal movement speed |
| IsGrounded    | Bool  | Whether player is on ground |
| VelocityY     | Float | Vertical velocity |

#### Create Transitions

**Idle ↔ Walk**:
1. Right-click Idle → **Make Transition** → Click Walk
2. Select the transition arrow
3. In Inspector, uncheck **Has Exit Time**
4. Add condition: `Speed` **Greater** `0.1`

5. Right-click Walk → **Make Transition** → Click Idle
6. Uncheck **Has Exit Time**
7. Add condition: `Speed` **Less** `0.1`

**Any State → Jump**:
1. Right-click **Any State** → **Make Transition** → Click Jump
2. Uncheck **Has Exit Time**
3. Add conditions:
   - `IsGrounded` **false**
   - `VelocityY` **Greater** `0.1`

**Jump → Idle/Walk**:
1. Right-click Jump → **Make Transition** → Click Idle
2. Uncheck **Has Exit Time**
3. Add condition: `IsGrounded` **true**

**Optional: Jump → Fall**:
If you have a fall animation:
1. Jump → Make Transition → Fall
2. Add condition: `VelocityY` **Less** `-0.1`
3. Fall → Make Transition → Idle
4. Add condition: `IsGrounded` **true**

### Step 4: Configure Player GameObject

1. **Select your player GameObject** in the Hierarchy

2. **Add Animator component** (if not already present):
   - Click **Add Component**
   - Search for **Animator**
   - Assign your Animator Controller to the **Controller** field

3. **Add PlayerAnimationController**:
   - Click **Add Component**
   - Search for **Player Animation Controller**
   - It will automatically find the Animator and PlayerMovement components

4. **Configure PlayerAnimationController**:
   - **Walk Speed Threshold**: `0.1` (minimum speed to trigger walk)
   - **Flip Sprite On Direction**: ✓ (checked to flip sprite when moving left)

### Step 5: Test Your Animations

1. **Enter Play Mode**
2. **Test each state**:
   - Stand still → Should play Idle
   - Move left/right → Should play Walk and flip sprite
   - Jump → Should play Jump
   - Land → Should return to Idle/Walk

## Animator Controller Visual Layout

```
┌─────────────┐
│  Any State  │
└──────┬──────┘
       │ (IsGrounded=false, VelocityY>0.1)
       ↓
┌─────────────┐     ┌─────────────┐
│    Idle     │ ←──→│    Walk     │
│  (default)  │     │             │
└──────┬──────┘     └─────────────┘
       │  Speed>0.1      Speed<0.1
       │
       │ (IsGrounded=true)
       ↓
┌─────────────┐
│    Jump     │
│             │
└─────────────┘
```

## Advanced Customization

### Adjust Transition Settings

For smoother transitions, adjust these settings on each transition:

- **Transition Duration**: `0.1` - `0.25` (shorter = snappier)
- **Transition Offset**: `0` (when to start the next animation)
- **Interruption Source**: `Current State` (allows interrupting animations)

### Add Animation Events

You can add events to animations for sound effects:

1. Open Animation window
2. Select an animation clip
3. Click the **Event** button (flag icon)
4. Add event at desired frame
5. Create a method in a script to handle the event

Example:
```csharp
public void PlayFootstepSound()
{
    // Play footstep audio
}
```

### Custom Animation Parameters

You can add more parameters for advanced animations:

```csharp
// In your custom script
PlayerAnimationController animController = GetComponent<PlayerAnimationController>();

// Trigger death animation
animController.SetAnimatorBool("IsDead", true);

// Trigger attack animation
animController.TriggerAnimation("Attack");
```

## Troubleshooting

### Animations Not Playing
- **Check**: Animator Controller is assigned to Animator component
- **Check**: Animation clips are properly created with sprite frames
- **Check**: Transitions have correct conditions
- **Check**: Parameters are spelled correctly (case-sensitive)

### Sprite Not Flipping
- **Check**: `Flip Sprite On Direction` is enabled in PlayerAnimationController
- **Check**: SpriteRenderer component exists on GameObject
- **Check**: Walk Speed Threshold is set appropriately

### Transitions Too Slow/Fast
- Adjust **Transition Duration** in Animator transitions
- Adjust animation **Sample Rate** in Animation window

### Jump Animation Not Triggering
- **Check**: `IsGrounded` parameter is being updated
- **Check**: Transition from Any State to Jump has correct conditions
- **Check**: PlayerMovement component is working correctly

## Performance Tips

1. **Use Animation Hashes**: The PlayerAnimationController already uses hashed parameter names for better performance

2. **Minimize Transitions**: Only create necessary transitions to reduce complexity

3. **Optimize Sprite Sheets**: Use texture atlases and compress sprites appropriately

## Example Animator Controller Settings

### Idle State
- **Speed**: 1
- **Loop**: ✓ (checked)

### Walk State
- **Speed**: 1
- **Loop**: ✓ (checked)

### Jump State
- **Speed**: 1
- **Loop**: ✗ (unchecked)

## Integration with Existing System

The PlayerAnimationController integrates seamlessly with your refactored components:

```
MainController (Orchestrator)
├── PlayerMovement (provides velocity & grounded state)
├── CollisionResolver
├── TilemapCollisionDetector
└── PlayerAnimationController (reads from PlayerMovement)
```

The animation controller automatically reads:
- `Velocity.x` → Updates `Speed` parameter
- `Velocity.y` → Updates `VelocityY` parameter
- `IsGrounded` → Updates `IsGrounded` parameter

No additional code needed in MainController!

## Next Steps

After setting up basic animations, you can:

1. Add more animation states (attack, hurt, death)
2. Implement animation events for sound effects
3. Create blend trees for smoother movement
4. Add particle effects triggered by animations
5. Implement animation layers for upper/lower body separation

## Resources

- Unity Animation Documentation: https://docs.unity3d.com/Manual/AnimationSection.html
- Unity Animator Controller: https://docs.unity3d.com/Manual/class-AnimatorController.html
- 2D Animation Tutorial: https://learn.unity.com/tutorial/introduction-to-sprite-animations

## Summary

You now have a complete animation system that:
- ✓ Automatically updates based on player movement
- ✓ Handles idle, walk, and jump animations
- ✓ Flips sprite based on direction
- ✓ Integrates with your refactored component system
- ✓ Is easy to extend with new animations

Just follow the steps above to set up your animator controller, and your cowboy will come to life!
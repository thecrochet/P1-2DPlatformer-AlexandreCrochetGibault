# P1-2DPlatformer-AlexandreCrochetGibault
Dawson IVGD - Scripting 2 - Assignment 1 - Alexandre Crochet Gibault


Alexandre Crochet-Gibault
Student id : 2545874

Controls Keyboard - GamePad
Activate Debug in Scene : F1
Movement : WASD - left joystick
Jump : Spacebar - A (south) button
Fire : enter - right Trigger - left mouse click


Physics approach
I used Rigidbody2D with velocity manipulation
Because it'is simple to tune for platformer feel (gravity, collision response, friction)

I used Physics2D overlapCircle for ground detetion at the character's base with LayerMask

I used Coyote timer for the jump and jump buffering time

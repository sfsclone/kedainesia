# Bug Fix: Customer Instantly Disappearing and Missing Smile Emote

The bug occurs because `CustomerManager.OnFoodServed` triggers a departure sequence immediately, which destroys the customer object before its internal "Smile" emote coroutine has a chance to run or complete.

## Project Overview
- **Game Title**: Kedainesia
- **High-Level Concept**: Restaurant management game focused on traditional Indonesian cuisine.
- **Key Systems**: Customer management, order processing, and emoji feedback.

## Bug Analysis
When the correct food is served:
1. `CustomerManager.OnFoodServed` is called.
2. It calls `controller.MarkAsServed()`, which starts the `LeaveHappily` coroutine on the customer.
3. `LeaveHappily` is supposed to show the "Smile" emoji and wait for `emojiDisplayDuration`.
4. However, `CustomerManager.OnFoodServed` *also* immediately starts `SpawnNextCustomerWithDelay()`.
5. `SpawnNextCustomerWithDelay()`'s first action is to `Destroy(currentCustomerInstance)`, which kills the customer object instantly, skipping the wait and the emoji display.

## Proposed Fix
Remove the redundant `StartCoroutine(SpawnNextCustomerWithDelay())` call in `CustomerManager.OnFoodServed`. The customer will handle its own departure sequence and signal the manager to spawn the next customer once it is done.

## Implementation Steps
### 1. Modify CustomerManager.cs
- **Description**: Remove the redundant coroutine call in the `OnFoodServed` method.
- **File**: `Assets/Scripts/CustomerManager.cs`
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: No

## Verification & Testing
### Automated Tests
- None (Visual/Runtime behavior).

### Manual Verification
1. Play the game and wait for a customer to spawn.
2. Prepare the correct recipe for the customer.
3. Serve the food to the customer.
4. **Verify**: The customer should show a "Smile" emoji.
5. **Verify**: The customer should remain visible for a short duration (1.5 seconds) while the emoji is shown.
6. **Verify**: After the duration, the customer should disappear.
7. **Verify**: After a short delay (2 seconds), the next customer should spawn.
8. **Verify**: The customer progress (e.g., "1/5") updates correctly.

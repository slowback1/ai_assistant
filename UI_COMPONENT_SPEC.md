# Donetick UI Component Specification

## Visual Layout

```
┌─────────────────────────────────────────────────────────┐
│  To-Do Tasks                                            │
│  ─────────────────────────────────────────────────────  │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │ 🗒️  Switch bedding               [Complete ✓]     │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │ 🗒️  Water plants                 [Complete ✓]     │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │ 🗒️  Clean kitchen                [Complete ✓]     │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## Component States

### Loading State
```
┌─────────────────────────────────────────────────────────┐
│  To-Do Tasks                                            │
│  ─────────────────────────────────────────────────────  │
│                                                          │
│                 Loading tasks...                        │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### Empty State (No Overdue Tasks)
```
┌─────────────────────────────────────────────────────────┐
│  To-Do Tasks                                            │
│  ─────────────────────────────────────────────────────  │
│                                                          │
│              No overdue tasks! 🎉                       │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### Error State
```
┌─────────────────────────────────────────────────────────┐
│  To-Do Tasks                                            │
│  ─────────────────────────────────────────────────────  │
│                                                          │
│     Failed to fetch overdue chores.                     │
│     Please try again later.                             │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### Completing State
```
┌─────────────────────────────────────────────────────────┐
│  To-Do Tasks                                            │
│  ─────────────────────────────────────────────────────  │
│                                                          │
│  ┌────────────────────────────────────────────────────┐ │
│  │ 🗒️  Switch bedding           [Completing... ⏳]    │ │
│  └────────────────────────────────────────────────────┘ │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

### Toast Notification (Success)
```
┌──────────────────────────────────┐
│  ✅ Chore completed successfully! │
└──────────────────────────────────┘
```

### Toast Notification (Error)
```
┌──────────────────────────────────┐
│  ❌ Failed to complete chore      │
└──────────────────────────────────┘
```

## CSS Styling

### Color Scheme
- **Section Background**: White (#FFFFFF)
- **Section Border**: Green (#4CAF50)
- **Card Background**: Light Gray (#F8F9FA)
- **Card Border**: Green Left Border (#4CAF50)
- **Button Background**: Green (#4CAF50)
- **Button Hover**: Dark Green (#45A049)
- **Button Disabled**: Gray (#CCCCCC)
- **Text Primary**: Dark Gray (#333)
- **Text Secondary**: Medium Gray (#666)
- **Error Text**: Red (#E63946)

### Spacing & Layout
- Section Padding: 2rem
- Card Padding: 1rem
- Gap Between Cards: 1rem
- Border Radius: 8px (section), 4px (cards)
- Box Shadow: 0 2px 8px rgba(0, 0, 0, 0.1)

### Typography
- Section Title: Large, Bold, Green Border Bottom
- Chore Name: 1.1rem, Medium Weight (#333)
- Loading/Error Text: 1.1rem, Center Aligned

### Interactions
- Card Hover: Slides right 4px
- Button Hover: Color darkens, scales 1.05x
- Button Click: Scales down to 0.98x
- Button Disabled: Gray, cursor not-allowed

## Responsive Behavior

The component is fully responsive and:
- Uses flexbox for layout
- Cards stack vertically
- Maintains readability on all screen sizes
- Touch-friendly button size on mobile

## Accessibility

- Semantic HTML structure
- Clear visual hierarchy
- Color contrast meets WCAG standards
- Loading states clearly indicated
- Error messages are descriptive

## Performance

- Polls every 5 minutes (300000ms)
- Prevents double-clicks during completion
- Optimistic UI updates (removes on completion)
- Minimal re-renders with Svelte reactivity

## Integration Points

1. **Dashboard Page**: Conditionally rendered based on DONETICK_ENABLED flag
2. **Toast System**: Uses existing ToastService for notifications
3. **Feature Flag**: Subscribes to FeatureFlagService
4. **API Service**: Uses DonetickApi extending BaseApi

## Data Flow

```
Component Mount
    ↓
Fetch Overdue Chores
    ↓
Display Chore List
    ↓
User Clicks "Complete"
    ↓
Disable Button (Completing...)
    ↓
Call API CompleteChore
    ↓
API Response
    ↓
├─ Success: Show Success Toast + Remove from List
└─ Error: Show Error Toast + Re-enable Button
```

## Feature Flag Behavior

When `DONETICK_ENABLED` is:
- **false**: Component not rendered (dashboard shows only story section)
- **true**: Component rendered above story section

Users can toggle this in `static/config/config.json` without code changes.

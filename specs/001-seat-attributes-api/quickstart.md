# Quickstart: Seat Attributes API

## Overview
This quickstart shows how to retrieve seat attributes and manage public notes for a seat.

## Endpoints
- GET `/seats/{model}/{seat}` – Retrieve attributes
- GET `/seats/{model}/{seat}/notes` – List notes
- POST `/seats/{model}/{seat}/notes` – Create note
- PUT `/seats/{model}/{seat}/notes` – Update note
- DELETE `/seats/{model}/{seat}/notes` – Delete note

## Sample Requests

### Retrieve Seat Attributes
```
GET /seats/A320/12A
```
Response (200):
```json
{
  "aircraftModel": "A320",
  "seatNumber": "12A",
  "position": "Window",
  "hasWindow": true,
  "powerAvailable": true,
  "powerType": "USB-C",
  "hasInSeatScreen": true,
  "experienceSummary": "Quiet row, good legroom"
}
```

### Create Note
```
POST /seats/A320/12A/notes
Content-Type: application/json

{ "text": "Great view during sunrise." }
```
Response (201): seat note object.

### Update Note
```
PUT /seats/A320/12A/notes
Content-Type: application/json

{ "id": 5, "text": "Still great; bring headphones." }
```
Response (200): updated note.

### Delete Note
```
DELETE /seats/A320/12A/notes?id=5
```
Response (204): no content.

## Error Examples
- 404 if seat not found.
- 400 if seatNumber invalid pattern.
- 400 if note text exceeds 500 characters.

## Observability
Logs include correlation id per request. Traces capture request span and DB query spans.

## Next Steps
Proceed with implementation tasks and writing tests first (TDD). Consult `openapi.yaml` for full contract.

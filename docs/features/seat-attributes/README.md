# Seat Attributes API

## Overview

The Seat Attributes API provides endpoints for retrieving aircraft seat information and managing public notes about specific seats. This API helps travelers discover seat characteristics (power outlets, window position, exit row status) and share experiences through community notes.

## Base URL

```
http://localhost:5000
```

*(Replace with your actual deployment URL in production)*

## Authentication

Currently, no authentication is required for public endpoints. Future versions may require API keys or OAuth tokens for note creation/modification.

## API Endpoints

### 1. Get Seat Attributes

Retrieve detailed information about a specific seat.

**Endpoint:** `GET /seats/{aircraftModel}/{seatNumber}`

**Parameters:**
- `aircraftModel` (string, path): Aircraft model identifier (e.g., "A320", "B737")
- `seatNumber` (string, path): Seat number in format: digits + letter A-F (e.g., "12A", "15F")

**Response:** `200 OK`
```json
{
  "aircraftModel": "A320",
  "seatNumber": "12A",
  "position": "Window",
  "hasWindow": true,
  "windowConfirmed": true,
  "isExitRow": false,
  "hasPower": true,
  "powerType": "USB-C"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid seat number format
- `404 Not Found`: Seat does not exist in database

**Example Request:**
```bash
curl -X GET "http://localhost:5000/seats/A320/12A" \
  -H "Accept: application/json"
```

---

### 2. Create Note

Add a public note for a specific seat.

**Endpoint:** `POST /seats/{aircraftModel}/{seatNumber}/notes`

**Parameters:**
- `aircraftModel` (string, path): Aircraft model identifier
- `seatNumber` (string, path): Seat number

**Request Body:**
```json
{
  "text": "Great legroom and easy access to overhead bins!"
}
```

**Constraints:**
- `text` must not exceed 500 characters
- Text will be trimmed of leading/trailing whitespace

**Response:** `201 Created`
```json
{
  "id": 1,
  "aircraftModel": "A320",
  "seatNumber": "12A",
  "text": "Great legroom and easy access to overhead bins!",
  "createdAt": "2025-10-29T10:30:00Z",
  "updatedAt": "2025-10-29T10:30:00Z"
}
```

**Error Responses:**
- `400 Bad Request`: Invalid seat number or text exceeds 500 characters
- `404 Not Found`: Seat does not exist (foreign key constraint)

**Example Request:**
```bash
curl -X POST "http://localhost:5000/seats/A320/12A/notes" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "text": "Great legroom and easy access to overhead bins!"
  }'
```

---

### 3. List Notes

Retrieve all notes for a specific seat, ordered by most recent first.

**Endpoint:** `GET /seats/{aircraftModel}/{seatNumber}/notes`

**Parameters:**
- `aircraftModel` (string, path): Aircraft model identifier
- `seatNumber` (string, path): Seat number

**Response:** `200 OK`
```json
[
  {
    "id": 2,
    "aircraftModel": "A320",
    "seatNumber": "12A",
    "text": "Power outlet works perfectly for laptop charging.",
    "createdAt": "2025-10-29T11:00:00Z",
    "updatedAt": "2025-10-29T11:00:00Z"
  },
  {
    "id": 1,
    "aircraftModel": "A320",
    "seatNumber": "12A",
    "text": "Great legroom and easy access to overhead bins!",
    "createdAt": "2025-10-29T10:30:00Z",
    "updatedAt": "2025-10-29T10:30:00Z"
  }
]
```

**Error Responses:**
- `400 Bad Request`: Invalid seat number format

**Example Request:**
```bash
curl -X GET "http://localhost:5000/seats/A320/12A/notes" \
  -H "Accept: application/json"
```

---

### 4. Update Note

Update the text of an existing note. Uses last-write-wins concurrency strategy.

**Endpoint:** `PUT /notes/{noteId}`

**Parameters:**
- `noteId` (integer, path): ID of the note to update

**Request Body:**
```json
{
  "text": "Updated: Power outlet works great, window view is excellent!"
}
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "aircraftModel": "A320",
  "seatNumber": "12A",
  "text": "Updated: Power outlet works great, window view is excellent!",
  "createdAt": "2025-10-29T10:30:00Z",
  "updatedAt": "2025-10-29T12:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request`: Text exceeds 500 characters
- `404 Not Found`: Note with specified ID does not exist

**Example Request:**
```bash
curl -X PUT "http://localhost:5000/notes/1" \
  -H "Content-Type: application/json" \
  -H "Accept: application/json" \
  -d '{
    "text": "Updated: Power outlet works great, window view is excellent!"
  }'
```

---

### 5. Delete Note

Delete a note by ID.

**Endpoint:** `DELETE /notes/{noteId}`

**Parameters:**
- `noteId` (integer, path): ID of the note to delete

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found`: Note with specified ID does not exist

**Example Request:**
```bash
curl -X DELETE "http://localhost:5000/notes/1" \
  -H "Accept: application/json"
```

---

## Data Models

### Seat Attributes

| Field | Type | Description |
|-------|------|-------------|
| aircraftModel | string | Aircraft model identifier (e.g., "A320") |
| seatNumber | string | Seat number (e.g., "12A") |
| position | string | Seat position: "Aisle", "Middle", or "Window" |
| hasWindow | boolean | True if seat has a window |
| windowConfirmed | boolean | Derived: `hasWindow && position == "Window"` |
| isExitRow | boolean | True if seat is in exit row |
| hasPower | boolean | True if power outlet available |
| powerType | string? | Type of power outlet (e.g., "USB-C", "AC") |

### Seat Note

| Field | Type | Description |
|-------|------|-------------|
| id | integer | Unique note identifier |
| aircraftModel | string | Related seat's aircraft model |
| seatNumber | string | Related seat number |
| text | string | Note content (max 500 characters) |
| createdAt | datetime | Creation timestamp (UTC) |
| updatedAt | datetime | Last update timestamp (UTC) |

---

## Error Handling

All error responses follow the Problem Details format (RFC 7807):

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid seat number format. Expected format: digits + letter A-F (e.g., '12A')",
  "traceId": "00-abc123...-def456...-00"
}
```

**Common HTTP Status Codes:**
- `200 OK`: Successful GET/PUT request
- `201 Created`: Successful POST request
- `204 No Content`: Successful DELETE request
- `400 Bad Request`: Validation error or malformed request
- `404 Not Found`: Resource does not exist
- `500 Internal Server Error`: Unexpected server error

---

## OpenAPI Documentation

Interactive API documentation is available via Scalar UI:

```
http://localhost:5000/scalar/v1
```

The OpenAPI specification can be accessed at:

```
http://localhost:5000/openapi/v1.json
```

---

## Integration Scenarios

For detailed integration scenarios and workflows, see [quickstart.md](../../../specs/001-seat-attributes-api/quickstart.md).

**Common Use Cases:**
1. **Seat Research Flow**: User searches for seat → retrieves attributes → reads community notes → makes booking decision
2. **Note Contribution Flow**: User books seat → experiences flight → creates note with feedback → helps future travelers
3. **Seat Comparison**: User retrieves multiple seats (e.g., 12A, 12B, 12C) → compares power/window attributes → selects preferred seat

---

## Development

### Running Locally

```bash
# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update --project src/seat-viewer-infrastructure --startup-project src/seat-viewer-api

# Run the API
dotnet run --project src/seat-viewer-api
```

### Sample Data

The development environment includes seed data:
- **A320 Seats**: 12A (window, USB-C), 12B (middle, no power), 12C (aisle, USB-C)
- **B737 Seats**: 15F (exit row, no window)

### Observability

See [observability.md](./observability.md) for details on:
- Structured logging with Serilog
- Distributed tracing with OpenTelemetry
- Correlation ID tracking
- Monitoring and alerting strategies

---

## Support

For issues, questions, or contributions, please refer to the project repository or contact the development team.


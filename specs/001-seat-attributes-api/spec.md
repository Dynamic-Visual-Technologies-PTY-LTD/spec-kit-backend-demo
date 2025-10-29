
# Feature Specification: Seat Attributes API

**Feature Branch**: `001-seat-attributes-api`  
**Created**: 2025-10-29  
**Status**: Draft  
**Input**: User description: "A simple backend API for one airline that accepts aircraft model and seat number, returns the seat’s attributes, and lets users create, update, and retrieve personal notes. The read endpoint provides seat position like aisle, middle, or window, confirms if a window seat actually has a window, indicates power availability and type, shows whether there is an in-seat screen, and summarizes expected experience such as legroom or noise. The write endpoints attach user notes to that exact seat and fetch them later. The notes should be public and not tied to a user."


## User Scenarios & Testing *(mandatory)*

### User Story 1 - Retrieve Seat Attributes (Priority: P1)

As a user, I want to provide an aircraft model and seat number and receive detailed attributes for that seat, so I can understand its position (aisle, middle, window), whether it has a window, power availability and type, in-seat screen presence, and a summary of the expected experience (legroom, noise, etc.).

**Why this priority**: This is the core value proposition—enabling users to make informed seat choices.

**Independent Test**: Can be fully tested by calling the read endpoint with various aircraft/seat combinations and verifying the returned attributes match known configurations.

**Acceptance Scenarios**:
1. **Given** a valid aircraft model and seat number, **When** the user requests seat attributes, **Then** the system returns all required details for that seat.
2. **Given** a window seat that does not have a window, **When** the user requests attributes, **Then** the system indicates no window is present.
3. **Given** a seat with power, **When** the user requests attributes, **Then** the system specifies the power type (e.g., AC, USB).
4. **Given** a seat with an in-seat screen, **When** the user requests attributes, **Then** the system indicates screen presence.
5. **Given** a seat with known experience notes (e.g., extra legroom), **When** the user requests attributes, **Then** the system includes a summary of the expected experience.

---

### User Story 2 - Add Public Note to Seat (Priority: P2)

As a user, I want to add a public note to a specific seat (by aircraft model and seat number), so that future users can see shared tips or experiences for that seat.

**Why this priority**: Enables community knowledge sharing and enriches the seat data for all users.

**Independent Test**: Can be fully tested by submitting a note for a seat and verifying it is retrievable by others for the same seat.

**Acceptance Scenarios**:
1. **Given** a valid aircraft model and seat number, **When** a user submits a note, **Then** the note is stored and associated with that seat.
2. **Given** a seat with existing notes, **When** a new note is added, **Then** all notes are retrievable for that seat.
3. **Given** a seat, **When** notes are requested, **Then** the system returns all public notes for that seat.

---

### User Story 3 - Update or Remove Public Note (Priority: P3)

As a user, I want to update or remove a public note for a seat, so that outdated or incorrect information can be corrected or deleted.

**Why this priority**: Maintains accuracy and relevance of shared seat notes.

**Independent Test**: Can be fully tested by updating or deleting a note and verifying the change is reflected for all users.

**Acceptance Scenarios**:
1. **Given** a seat with an existing note, **When** a user updates the note, **Then** the updated note is shown for that seat.
2. **Given** a seat with an existing note, **When** a user deletes the note, **Then** the note is no longer returned for that seat.

---

### Edge Cases

- What happens when an invalid aircraft model or seat number is provided?
- How does the system handle requests for seats with no available data?
- What if a note is submitted that exceeds the allowed length?
- How are simultaneous updates to the same note handled?


## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST accept aircraft model and seat number as input and return all seat attributes (position, window presence, power availability/type, in-seat screen, experience summary).
- **FR-002**: System MUST allow users to submit public notes for a specific seat (aircraft model + seat number).
- **FR-003**: System MUST allow retrieval of all public notes for a specific seat.
- **FR-004**: System MUST allow updating or deleting a public note for a seat.
- **FR-005**: System MUST validate aircraft model and seat number for all operations and return clear errors for invalid input.
- **FR-006**: System MUST handle requests for seats with no data gracefully, returning an appropriate message.
- **FR-007**: System MUST enforce a maximum note length of [NEEDS CLARIFICATION: What is the maximum allowed note length?]
- **FR-008**: System MUST ensure notes are public and not tied to any user identity.
- **FR-009**: System MUST prevent simultaneous conflicting updates to the same note (e.g., last-write-wins or reject concurrent edits) [NEEDS CLARIFICATION: What is the desired conflict resolution strategy for simultaneous note updates?]


### Key Entities

- **Seat**: Represents a specific seat on a given aircraft model. Key attributes: aircraft model, seat number, position (aisle/middle/window), window presence, power availability/type, in-seat screen, experience summary.
- **SeatNote**: Represents a public note attached to a specific seat. Key attributes: aircraft model, seat number, note text, timestamp, note ID.


## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can retrieve seat attributes for any valid seat in under 2 seconds.
- **SC-002**: 95% of seat attribute queries return correct and complete information as verified against airline data.
- **SC-003**: 100% of submitted notes are retrievable for the correct seat within 5 seconds of submission.
- **SC-004**: No user is able to submit a note exceeding the maximum allowed length.
- **SC-005**: 100% of invalid seat or aircraft inputs result in clear, actionable error messages.
- **SC-006**: 90% of users surveyed report that seat notes are helpful in making seat choices.


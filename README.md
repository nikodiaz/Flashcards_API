# Vocabulary Flashcards

**Description:** Allows users to study vocabulary at scheduled intervals, reinforcing long-term memory.

# API Objectives

- Allow users to:
    - Create new flashcards with English vocabulary.
    - Query their flashcards and view their progress status (level within the Leitner system).
    - Update flashcard information (such as its meaning or usage example).
    - Delete flashcards.
- Implement spaced repetition logic to optimize word review time.

## Database Structure

### User

| **Property** | **Type** | **Description** |
| --- | --- | --- |
| ID | GUID | Unique identifier |
| Username | String | User's name |
| Password | String | Account password |
| Email | String | User's email |
| Flashcards | List | Flashcards created by the user |

### Flashcard

| **Property** | **Type** | **Description** |
| --- | --- | --- |
| ID | GUID | Unique identifier |
| UserID | GUID | Unique identifier of the user who created the flashcard |
| Word | String | The word in English |
| Definition | String | Definition or meaning of the word |
| Example | String | Example of usage in a sentence |
| Level | Int | Repetition level representing the*box*of the Leitner system |
| Next_review_date | DateTime | Date of the next review |

## Endpoints

- **POST** `/flashcards`: Create a new flashcard.
- **GET** `/flashcards`: Get all flashcards.
- **GET** `/flashcards/{id}`: Get a flashcard by its ID.
- **PUT** `/flashcards/{id}`: Update an existing flashcard.
- **DELETE** `/flashcards/{id}`: Delete a flashcard by its ID.
- **POST** `/flashcards/{id}/review`: Mark a flashcard as reviewed.
- **GET** `/flashcards/due`: Get flashcards due for review.

## Spaced Repetition Algorithm (Leitner Method)

- When creating a flashcard, it is assigned to level 1 (daily review).
- If the answer is correct, flashcards move to the next level with a longer interval.
    - Level 1: Daily review.
    - Level 2: Review after 3 days.
    - Level 3: Weekly review.
    - Level 4: Monthly review.
- If the answer is incorrect, it returns to level 1 to reinforce memorization.
- If the answer is average, it stays at the current level.

# UC-01: Register an account

**Actor:** a visitor who doesn't have an account yet
**Goal:** create a personal account to be able to sign in later
**Preconditions:** none; the visitor is not signed in
**Postconditions:** a new account exists with the visitor's name, surname and email, and the password is stored only as a hash. The visitor is _not_ signed in yet.

## What happens, step by step

| #   | What happens                                                                                    | Layer                                          |
| --- | ----------------------------------------------------------------------------------------------- | ---------------------------------------------- |
| 1   | The visitor fills in name, surname, email and password, and sends the form.                     | Api                                            |
| 2   | The API receives the request and hands the data to the "register account" use case.             | Api → Application                              |
| 3   | The system checks that the password follows the password rules (see BR-2).                      | Application                                    |
| 4   | The system checks that no account already uses this email (compared in lowercase).              | Application, through the account repository    |
| 5   | The system turns the password into a hash. The plain password is never stored.                  | Infrastructure, through the password hasher    |
| 6   | The system creates the account. It cleans up the name and email and refuses an invalid account. | Domain (`Account`)                             |
| 7   | The system saves the account.                                                                   | Infrastructure, through the account repository |
| 8   | The API answers "created" with the new account's id.                                            | Api                                            |

## When things go differently

- **3a. The password is too weak:** the system rejects the request and explains the rule. Nothing is saved.
- **4a. The email is already used:** the system rejects the request with "this email is already registered". Nothing is saved.
- **6a. Name, surname or email is empty, or the email is invalid:** the account refuses to be created, and the API answers "bad request". Nothing is saved.
- **7a. Someone else registers the same email at the same moment:** both pass step 4, but the unique index in the database lets only one succeed. The second visitor gets the same answer as in 4a.
- **7b. The database is unavailable:** the system answers with a server error, saves nothing, and logs the problem.

## Business rules

- BR-1: An email identifies one account only, and it is case-insensitive.
- BR-2: A password has at least 8 characters.
- BR-3: The plain password is never stored or logged.

## Acceptance tests

- Given no account with `a@x.com`, when a visitor registers with `A@X.com`, then an account with email `a@x.com` exists.
- Given an account with `a@x.com`, when someone registers with `A@X.com`, then the request is rejected and there is still one account.

## Open questions

- Does registering also create a company, or is that a separate use case after signing in? _(My suggestion: separate, with its own use case.)_
- Should the answer to 4a reveal that the email exists? It's friendlier, but it lets an attacker discover who has an account. Many apps accept this trade-off for registration.

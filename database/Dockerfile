FROM flyway/flyway:11-alpine

LABEL org.opencontainers.image.title="PersonalFinance database migrations"
LABEL org.opencontainers.image.description="Flyway image containing the PersonalFinance database migrations."

COPY database/migrations /flyway/sql

CMD ["info"]

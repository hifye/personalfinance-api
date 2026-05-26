FROM gradle:8.10-jdk21

# Download Flyway CLI (versão 11)
RUN curl -L https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/11.0.0/flyway-commandline-11.0.0-linux-x64.tar.gz | tar xz && \
    mv flyway-11.0.0 /flyway && \
    rm -rf /app

# Download PostgreSQL JDBC driver (latest)
RUN mkdir -p /flyway/drivers && \
    curl -o /flyway/drivers/postgresql-42.7.3.jar https://jdbc.postgresql.org/download/postgresql-42.7.3.jar

COPY migrations /flyway/sql

ENV PATH="/flyway:$PATH"

ENTRYPOINT ["flyway", "-baselineOnMigrate=true", "migrate"]

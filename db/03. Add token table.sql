CREATE SEQUENCE identity_user_token_seq
    INCREMENT 1
    START 19
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

CREATE TABLE identity_user_token
(
    identity_user_token_id integer NOT NULL DEFAULT nextval('identity_user_token_seq'::regclass),
    login_provider character varying(255) COLLATE pg_catalog."default",
    token_name character varying(255) COLLATE pg_catalog."default",
    token_value character varying(255) COLLATE pg_catalog."default",
    user_id integer,
    CONSTRAINT identity_user_token_pkey PRIMARY KEY (identity_user_token_id),
    CONSTRAINT identity_user_token_unique_key UNIQUE (login_provider, token_name, user_id),
    CONSTRAINT fk_identity_user_token_user FOREIGN KEY (user_id)
        REFERENCES public.identity_user (identity_user_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;
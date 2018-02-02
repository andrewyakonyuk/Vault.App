-- Table: public.identity_user
CREATE SEQUENCE identity_user_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

CREATE TABLE identity_user
(
    identity_user_id integer NOT NULL NOT NULL DEFAULT nextval('identity_user_seq'::regclass),
    access_failed_count integer,
    concurrency_stamp character varying(255) COLLATE pg_catalog."default",
    email character varying(255) COLLATE pg_catalog."default",
    email_confirmed boolean,
    lockout_enabled boolean,
    lockout_end timestamp without time zone,
    normalized_email character varying(255) COLLATE pg_catalog."default",
    normalized_user_name character varying(255) COLLATE pg_catalog."default",
    password_hash character varying(255) COLLATE pg_catalog."default",
    security_stamp character varying(255) COLLATE pg_catalog."default",
    user_name character varying(255) COLLATE pg_catalog."default",
    CONSTRAINT identity_user_pkey PRIMARY KEY (identity_user_id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

-- Table: public.identity_user_login

CREATE SEQUENCE identity_user_login_seq
    INCREMENT 1
    START 14
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

DROP TABLE identity_user_login;

CREATE TABLE identity_user_login
(
    identity_user_login_id integer NOT NULL DEFAULT nextval('identity_user_login_seq'::regclass),
    login_provider character varying(255) COLLATE pg_catalog."default",
    provider_display_name character varying(255) COLLATE pg_catalog."default",
    provider_key character varying(255) COLLATE pg_catalog."default",
    user_id integer,
    CONSTRAINT identity_user_login_pkey PRIMARY KEY (identity_user_login_id),
    CONSTRAINT fk_identity_user_login_identity_user FOREIGN KEY (user_id)
        REFERENCES public.identity_user (identity_user_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT fk_identity_user_login_user FOREIGN KEY (user_id)
        REFERENCES public.identity_user (identity_user_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

-- Table: public.identity_user_claim

CREATE SEQUENCE identity_user_claim_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

CREATE TABLE public.identity_user_claim
(
    identity_user_claim_id integer NOT NULL nextval('identity_user_claim_seq'::regclass),
    claim_type character varying(255) COLLATE pg_catalog."default",
    claim_value character varying(255) COLLATE pg_catalog."default",
    user_id integer,
    CONSTRAINT identity_user_claim_pkey PRIMARY KEY (identity_user_claim_id),
    CONSTRAINT fk_identity_user_claim_identity_user FOREIGN KEY (user_id)
        REFERENCES public.identity_user (identity_user_id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

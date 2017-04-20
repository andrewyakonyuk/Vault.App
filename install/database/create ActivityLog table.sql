CREATE SEQUENCE public.activitylogs_checkpointnumber_seq
    INCREMENT 1
    START 12385
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.activitylogs_checkpointnumber_seq
    OWNER TO postgres;

-- Table: public.activitylogs

-- DROP TABLE public.activitylogs;

CREATE TABLE public.activitylogs
(
    bucketid character varying(128) COLLATE pg_catalog."default" NOT NULL,
    streamid character varying(128) COLLATE pg_catalog."default" NOT NULL,
    id character varying(255) COLLATE pg_catalog."default" NOT NULL,
    verb character varying(40) COLLATE pg_catalog."default" NOT NULL,
    target character varying(255) COLLATE pg_catalog."default",
    provider character varying(40) COLLATE pg_catalog."default" NOT NULL,
    url character varying(1024) COLLATE pg_catalog."default",
    published timestamp with time zone NOT NULL,
    metabag bytea,
    checkpointnumber integer NOT NULL DEFAULT nextval('activitylogs_checkpointnumber_seq'::regclass),
    actor character varying(255) COLLATE pg_catalog."default" NOT NULL,
    title character varying COLLATE pg_catalog."default",
    content character varying COLLATE pg_catalog."default",
    CONSTRAINT activitylogs_pkey PRIMARY KEY (checkpointnumber)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.activitylogs
    OWNER to postgres;
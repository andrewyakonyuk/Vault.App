-- Table: public.objects

CREATE TABLE objects
(
    id character varying COLLATE pg_catalog."default" NOT NULL,
    type character varying[] COLLATE pg_catalog."default" NOT NULL,
    attachment jsonb,
    "attributedTo" jsonb,
    audience jsonb,
    context jsonb,
    generator character varying COLLATE pg_catalog."default",
    "inReplyTo" jsonb,
    location jsonb,
    tag character varying[] COLLATE pg_catalog."default",
    updated timestamp with time zone,
    url character varying[] COLLATE pg_catalog."default",
    annotations jsonb,
    name character varying COLLATE pg_catalog."default",
    "nameMap" jsonb,
    content character varying COLLATE pg_catalog."default",
    "contentMap" jsonb,
    "endTime" timestamp with time zone,
    image jsonb,
    preview json,
    "startTime" timestamp with time zone,
    summary character varying COLLATE pg_catalog."default",
    "summaryMap" jsonb,
    "mediaType" character varying COLLATE pg_catalog."default",
    published timestamp with time zone,
    CONSTRAINT objects_pkey PRIMARY KEY (id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

-- Table: public.activities

CREATE SEQUENCE activity_checkpointtoken_seq
    INCREMENT 1
    START 2109
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

CREATE TABLE activities
(
	actor jsonb,
	instrument jsonb,
	object jsonb,
	origin jsonb,
	result jsonb,
	target jsonb,
    "streamId" character varying(128) COLLATE pg_catalog."default" NOT NULL,
    bucket character varying(128) COLLATE pg_catalog."default" NOT NULL,
    "checkpointToken" bigint NOT NULL DEFAULT nextval('activity_checkpointtoken_seq'::regclass)
)
    INHERITS (public.objects)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

-- Table: public.board

CREATE SEQUENCE board_seq
    INCREMENT 1
    START 3
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

CREATE TABLE board
(
    board_id integer NOT NULL,
    name character varying(255) COLLATE pg_catalog."default",
    owner_id integer,
    raw_query character varying(255) COLLATE pg_catalog."default",
    published timestamp without time zone,
    CONSTRAINT board_pkey PRIMARY KEY (board_id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;
CREATE TABLE `quotes` (
  `QuoteId` double NOT NULL AUTO_INCREMENT,
  `guid` varchar(37) COLLATE latin1_danish_ci DEFAULT NULL,
  `QuoteText` varchar(15000) COLLATE latin1_danish_ci DEFAULT NULL,
  `Comment` varchar(6000) COLLATE latin1_danish_ci DEFAULT NULL,
  `Approved` double DEFAULT NULL,
  `Type` double DEFAULT NULL,
  PRIMARY KEY (`QuoteId`)
) ENGINE=MyISAM AUTO_INCREMENT=2024 DEFAULT CHARSET=latin1


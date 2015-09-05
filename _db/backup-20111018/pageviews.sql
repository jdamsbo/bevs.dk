-- phpMyAdmin SQL Dump
-- version 3.4.0
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Generation Time: Oct 18, 2011 at 09:39 AM
-- Server version: 5.1.35
-- PHP Version: 5.2.6

SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Database: `bevs_dk_db`
--

-- --------------------------------------------------------

--
-- Table structure for table `pageviews`
--

CREATE TABLE IF NOT EXISTS `pageviews` (
  `Page` varchar(1000) DEFAULT NULL,
  `PageViews` int(16) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Dumping data for table `pageviews`
--

INSERT INTO `pageviews` (`Page`, `PageViews`) VALUES
('/thyboe/Default.aspx', 307793),
('/jj/Default.aspx', 889837),
('/navne/Default.aspx', 89933),
('/chuck/Default.aspx', 92997),
('/hmmm/Default.aspx', 162916),
('/bevs.dk/thyboe/default.aspx', 45),
('/bevs.dk/navne/Default.aspx', 14),
('/bevs.dk/jj/Default.aspx', 51),
('/bevs.dk/chuck/default.aspx', 7),
('/bevs.dk/hmmm/default.aspx', 1);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;

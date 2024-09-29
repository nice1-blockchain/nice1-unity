//  To parse this JSON data, first install
//
//      Boost     http://www.boost.org
//      json.hpp  https://github.com/nlohmann/json
//
//  Then include this file, and then do
//
//     Welcome data = nlohmann::json::parse(jsonString);

#pragma once

#include <nlohmann/json.hpp>
#include <stdexcept>
#include <regex>

namespace quicktype {
	using nlohmann::json;

#ifndef NLOHMANN_UNTYPED_quicktype_HELPER
#define NLOHMANN_UNTYPED_quicktype_HELPER
	inline json get_untyped(const json& j, const char* property) {
		if (j.find(property) != j.end()) {
			return j.at(property).get<json>();
		}
		return json();
	}

	inline json get_untyped(const json& j, std::string property) {
		return get_untyped(j, property.data());
	}

	inline const std::string AddBarBeforeDoubleQuote(std::string s) {
		std::string other = "\\\"";

		std::string::size_type pos = 0;

		while (s.find("\"", pos) != std::string::npos) {
			std::string::size_type found = s.find("\"", pos);
			s.replace(found, 1, other);
			pos = found + 2;
		}

		return s;
	}
#endif

	class Idata {
	public:
		Idata() = default;
		virtual ~Idata() = default;

	private:
		std::string idata;

	public:
		const std::string& get_complete() const { return idata; }
		void set(const std::string& value) { this->idata = value; }
		const std::string get_name() const {
			std::string name = idata.substr(9, idata.size() - 11);
			std::string::size_type found = name.find("\"");

			if (found != std::string::npos)
				name = name.substr(0, found);

			return name;
		}
	};

	class Row {
	public:
		Row() = default;
		virtual ~Row() = default;

	private:
		std::string author;
		std::string category;
		std::vector<nlohmann::json> container;
		std::vector<nlohmann::json> containerf;
		std::string id;
		Idata idata;
		std::string mdata;
		std::string owner;

	public:
		const std::string& get_author() const { return author; }
		const std::string& get_mutable_author() { return author; }
		void set_author(const std::string& value) { this->author = value; }

		const std::string& get_category() const { return category; }
		std::string& get_mutable_category() { return category; }
		void set_category(const std::string& value) { this->category = value; }

		const std::vector<nlohmann::json>& get_container() const { return container; }
		std::vector<nlohmann::json>& get_mutable_container() { return container; }
		void set_container(const std::vector<nlohmann::json>& value) { this->container = value; }

		const std::vector<nlohmann::json>& get_containerf() const { return containerf; }
		std::vector<nlohmann::json>& get_mutable_containerf() { return containerf; }
		void set_containerf(const std::vector<nlohmann::json>& value) { this->containerf = value; }

		const std::string& get_id() const { return id; }
		std::string& get_mutable_id() { return id; }
		void set_id(const std::string& value) { this->id = value; }

		const Idata get_idata() const { return idata; }
		Idata get_mutable_idata() { return idata; }
		void set_idata(const std::string value) { this->idata.set(value); }

		const std::string& get_mdata() const { return mdata; }
		std::string& get_mutable_mdata() { return mdata; }
		void set_mdata(const std::string& value) { this->mdata = value; }

		const std::string& get_owner() const { return owner; }
		std::string& get_mutable_owner() { return owner; }
		void set_owner(const std::string& value) { this->owner = value; }
	};

	class JsonDataContainer {
	public:
		JsonDataContainer() = default;
		virtual ~JsonDataContainer() = default;

	private:
		bool more;
		std::string next_key;
		std::vector<Row> rows;

	public:
		const bool& get_more() const { return more; }
		bool& get_mutable_more() { return more; }
		void set_more(const bool& value) { this->more = value; }

		const std::string& get_next_key() const { return next_key; }
		std::string& get_mutable_next_key() { return next_key; }
		void set_next_key(const std::string& value) { this->next_key = value; }

		const std::vector<Row>& get_rows() const { return rows; }
		std::vector<Row>& get_mutable_rows() { return rows; }
		void set_rows(const std::vector<Row>& value) { this->rows = value; }
	};
}

namespace quicktype {
	void from_json(const json& j, Row& x);
	void to_json(json& j, const Row& x);

	void from_json(const json& j, JsonDataContainer& x);
	void to_json(json& j, const JsonDataContainer& x);

	inline void from_json(const json& j, Row& x) {
		x.set_author(j.at("author").get<std::string>());
		x.set_category(j.at("category").get<std::string>());
		x.set_container(j.at("container").get<std::vector<nlohmann::json>>());
		x.set_containerf(j.at("containerf").get<std::vector<nlohmann::json>>());
		x.set_id(j.at("id").get<std::string>());
		x.set_idata(j.at("idata").get<std::string>());
		x.set_mdata(j.at("mdata").get<std::string>());
		x.set_owner(j.at("owner").get<std::string>());
	}

	inline void to_json(json& j, const Row& x) {
		j = json::object();
		j["author"] = x.get_author();
		j["category"] = x.get_category();
		j["container"] = x.get_container();
		j["containerf"] = x.get_containerf();
		j["id"] = x.get_id();
		j["idata"] = x.get_idata().get_complete();
		j["mdata"] = x.get_mdata();
		j["owner"] = x.get_owner();
	}

	inline void from_json(const json& j, JsonDataContainer& x) {
		x.set_more(j.at("more").get<bool>());
		x.set_next_key(j.at("next_key").get<std::string>());
		x.set_rows(j.at("rows").get<std::vector<Row>>());
	}

	inline void to_json(json& j, const JsonDataContainer& x) {
		j = json::object();
		j["more"] = x.get_more();
		j["next_key"] = x.get_next_key();
		j["rows"] = x.get_rows();
	}
}

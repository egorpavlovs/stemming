require "arachnid2"
require "nokogiri"

require 'rubygems'
require 'lingua/stemmer'
require 'fileutils'
require "lemmatizer"

DOCS_HOME_PATH = 'docs'
L_DOCS_HOME_PATH = 'lemm_docs'

class WebService
  class << self

    def init_method()
      words = %w(architecture art bar cabaret concert creative creativity cultural culture design  festival gallery gastronomy heritage historic history local cousine mansion monument museum music nationality nightclub nightlife oenology palace party restaurant sightseeing theatre university customs tradition traditional excursion)

      # "barcelonaconventionbureau"=>"http://www.barcelonaconventionbureau.com",уже есть данные
      # data present
      # urls_hash = {
      #   "parisinfo"=>"http://en.convention.parisinfo.com"
      # }
      urls_hash = {
        "barcelonaconventionbureau"=>"http://www.barcelonaconventionbureau.com",
        "parisinfo"=>"http://en.convention.parisinfo.com",
        "vienna"=>"https://www.vienna.convention.at/en",
        "visitberlin"=>"https://convention.visitberlin.de/en",
        "london"=>"https://conventionbureau.london/",
        "visitsingapore"=>"https://www.visitsingapore.com/mice/en/about-us/about-secb/",
        "miceseoul"=>"http://www.miceseoul.com/",
        "mehongkong"=>"https://mehongkong.com",
        "businesseventsthailand"=>"https://www.businesseventsthailand.com",
        "tcvb"=>"https://www.tcvb.or.jp/en/",
        "buenosairesbureau"=>"http://www.buenosairesbureau.com/en",
        "congresmtl"=>"https://congresmtl.com/en/",
        "limaconvention"=>"http://limaconvention.com/en/",
        "seetorontonow"=>"http://partners.seetorontonow.com/",
        "joburgtourism"=>"http://listings.joburgtourism.com",
        "dubaiconventionbureau"=>"http://dubaiconventionbureau.com/",
        "visitabudhabi"=>"https://visitabudhabi.ae/en/abu.dhabi.convention.bureau.aspx"
      }


      # urls_hash.each do |folder_name, url|
      #   puts folder_name
      #   puts url
      #   web_crawler(url, folder_name)
      #   puts "crawled"
      #   lemming(folder_name)
      #   puts "lemming"
      # end

      result_array = urls_hash.map do |folder_name, url|
        find_words_count(folder_name, words, url)
      end
      puts result_array
    end

    def find_words_count(folder_name, words, url)
      result = []
      all_paths = Dir["#{L_DOCS_HOME_PATH}/#{folder_name}/*/*/*"]

      words.each do |word|
        count = 0
        all_paths.each do |file_path|
          words_from_file = File.read(file_path).split("\n")
          count += words_from_file.count{ |element| element.match(word) }
        end
        result << [word, count]
      end
      "#{url}: #{result}"
    end


    def lemming(folder_name)
      all_paths = Dir["#{DOCS_HOME_PATH}/#{folder_name}/*/*/"]
      words = all_paths.map do |path|
        print '*'
        content = File.open([path, "content.txt"].join("/"), "r", encoding: 'ISO-8859-1:UTF-8'){ |f| f.read }
        stop_words = File.open("stop_words", "r"){ |f| f.read }.split("\n")

        lem = Lemmatizer.new
        tokenize_words_lemm = tokenize(content, stop_words).map { |word| lem.lemma(word) unless word.nil? }.compact

        l_path = FileUtils.mkdir_p([L_DOCS_HOME_PATH, path.split("/").drop(1)].flatten.join("/"))
        puts l_path
        tokenize_words_lemm.each{ |word|
          File.open([l_path, "content.txt"].flatten.join("/"), "a") {|file| file.puts(word)}
        }
      end
    end

    def tokenize(content, stop_words)
      numbers = content.scan(/\d+/)
      result = content.split(/\W+/) - numbers
      result.map{ |word| word.downcase if word.size > 1 && word.gsub(/\d+/,"") == word && !stop_words.include?(word) }
    end

    def web_crawler(url, folder_name)
      spider = Arachnid2.new(url)
      responses = []
      doc_dir = "docs"

      spider.crawl(max_urls: 2000, timeout: 10, time_box: 1000) { |response|
        responses << Nokogiri::HTML(response.body)
        print '*'
      }

      responses.each_with_index do |response, index|
        response_directory_name = "response_#{index}"
        response.elements.each_with_index do |element, i|
          element_directory_name = "element_#{index}"
          path = [doc_dir, folder_name, response_directory_name, element_directory_name].join('/')
          # Dir.mkdir(path) unless Dir.exists?(path)
          FileUtils.mkdir_p(path)
          element.search("//style|//script").remove
          # write all links to file
          links = element.xpath('//a').map {|element| element["href"]}.compact.select{ |str| str.start_with?("https") }
          File.open([path, "links.txt"].join("/"), "w") {|file| file.puts(links)}

          # write text content
          File.open([path, "content.txt"].join("/"), "w") {|file| file.puts(element.content)}
        end
      end
    end
  end
end

words = %w(architecture art bar cabaret concert creative creativity cultural culture design  festival gallery gastronomy heritage historic history local cousine mansion monument museum music nationality nightclub nightlife oenology palace party restaurant sightseeing theatre university customs tradition traditional excursion)
folder_name = "barcelonaconventionbureau"
url = "http://www.barcelonaconventionbureau.com"
# puts WebService.find_words_count(folder_name, words, url)

WebService.init_method()
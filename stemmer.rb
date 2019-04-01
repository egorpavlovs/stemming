require 'rubygems'
require 'lingua/stemmer'
require 'fileutils'

DOCS_HOME_PATH = 'docs'
S_DOCS_HOME_PATH = 'stemming_docs'

class Stemmer
  class << self
    def all_docs
      all_paths = Dir["#{DOCS_HOME_PATH}/*/*/"]
      words = all_paths.map do |path|
        content = File.open([path, "content.txt"].join("/"), "r"){ |f| f.read }
        stop_words = File.open("stop_words", "r"){ |f| f.read }.split("\n")

        tokenize_words = tokenize(content, stop_words).map { |word| Lingua.stemmer(word) unless word.nil? }.compact
        s_path = FileUtils.mkdir_p([S_DOCS_HOME_PATH, path.split("/").drop(1)].flatten.join("/"))
        puts s_path
        tokenize_words.uniq.each{ |word|
          File.open([s_path, "content.txt"].flatten.join("/"), "a") {|file| file.puts(word)}
        }
        FileUtils.cp([path, 'links.txt'].join('/'), [s_path, "links.txt"].join("/"))
      end
    end

    def tokenize(content, stop_words)
      numbers = content.scan(/\d+/)
      result = content.split(/\W+/) - numbers
      result.map{ |word| word.downcase if word.size > 1 && word.gsub(/\d+/,"") == word && !stop_words.include?(word) }
    end

  end
end

s = Stemmer.all_docs
puts "All stemming! Check #{S_DOCS_HOME_PATH}/"